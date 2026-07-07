using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameServer : NetworkBehaviour, IServerRpcSender
{
    public static GameServer Instance;

    private readonly ConcurrentDictionary<ulong, int> clientToPlayerId = new ConcurrentDictionary<ulong, int>();
    private int nextPlayerIndex = 1;
    private ServerGameModel serverModel;
    private ServerPresenter serverPresenter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            serverModel = new ServerGameModel();
            serverPresenter = new ServerPresenter(serverModel, DeckManager.Instance, this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void Start()
    {
        StartCoroutine(EnsureClientMvpAfterNetworkReady());
    }

    private IEnumerator EnsureClientMvpAfterNetworkReady()
    {
        yield return null;
        if (NetworkManager.Singleton == null) yield break;
        if (!NetworkManager.Singleton.IsClient) yield break;
        GamePresenter.EnsureClientStack();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        int assigned = nextPlayerIndex;
        nextPlayerIndex++;
        if (nextPlayerIndex > 3) nextPlayerIndex = 1;

        clientToPlayerId[clientId] = assigned;
        Debug.Log($"[Server] 为客户端 {clientId} 分配玩家编号 {assigned}");

        // 仅向刚连接的客户端发送其分配的玩家编号
        var clientParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } } };
        AssignPlayerIdClientRpc(assigned, clientParams);
    }

    [ClientRpc]
    private void AssignPlayerIdClientRpc(int playerId, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(NotifyPlayerIdAssigned(playerId));
    }

    private IEnumerator NotifyPlayerIdAssigned(int playerId)
    {
        yield return new WaitForSeconds(0.1f);
        NetworkService.Instance.NotifyPlayerIdAssigned(playerId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (clientToPlayerId.TryRemove(clientId, out int removed))
        {
            Debug.Log($"[Server] 客户端 {clientId} 断开连接，释放玩家编号 {removed}");
            if (removed >= 1 && removed <= 3) serverModel.PlayerPrepared[removed] = false;
        }
    }

    private bool ValidateSender(ServerRpcParams rpcParams, int playerId, out ulong sender)
    {
        sender = rpcParams.Receive.SenderClientId;
        if (!clientToPlayerId.TryGetValue(sender, out int mapped) || mapped != playerId)
        {
            Debug.LogWarning($"[Server] 发送方校验失败 sender={sender} playerId={playerId} mapped={clientToPlayerId.GetValueOrDefault(sender, -1)}");
            return false;
        }

        return true;
    }
    /// <summary>
    /// 客户端准备就绪，通知服务端。使用 ServerRpcParams 验证发送方身份。
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="rpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void CheckStartedServerRpc(int playerId, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!ValidateSender(rpcParams, playerId, out ulong sender)) return;

        Debug.Log($"[Server] 玩家准备就绪 sender={sender}, playerId={playerId}");
        serverPresenter.OnPlayerPrepared(playerId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChooseLordServerRpc(int currentPlayerIndex, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!ValidateSender(rpcParams, currentPlayerIndex, out _)) return;

        if (!serverPresenter.OnRefuseLord(currentPlayerIndex))
        {
            Debug.Log("叫地主处理失败，已忽略");
            return;
        }
    }
    [ClientRpc]
    public void NotCalledLordClientRpc()
    {
        SoundService.Instance.PlayNotCallLandlord();
    }
    [ClientRpc]
    public void ChooseLordClientRpc(int playerIndex)
    {
        NetworkService.Instance.NotifyBiddingTurn(playerIndex);
    }
    /// <summary>
    /// 玩家叫地主。服务端验证身份后，由 ServerPresenter 处理叫地主逻辑。
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="rpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void SetLordIndexServerRpc(int playerIndex, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!ValidateSender(rpcParams, playerIndex, out ulong sender)) return;
        Debug.Log($"[Server] 玩家 {playerIndex} 叫地主，sender={sender}");
        serverPresenter.OnChooseLord(playerIndex);
    }

    /// <summary>
    /// 定向发牌：每个客户端只收到自己的手牌（定向 ClientRpc），
    /// 全员收到底牌（背面）和各玩家余牌数。防止全量卡牌泄露。
    /// </summary>
    public void DeliverCardsForAllPlayers(ServerGameModel model)
    {
        // 1. 定向发送：每个客户端只拿到自己的手牌
        foreach (var kvp in clientToPlayerId)
        {
            ulong clientId = kvp.Key;
            int playerIndex = kvp.Value;
            var cards = model.GetPlayerCards(playerIndex);
            string json = JsonUtility.ToJson(new CardListWrapper { cards = cards });
            var clientParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            DeliverMyCardsClientRpc(json, clientParams);
        }

        // 2. 全员广播：底牌（背面朝上，地主确认后翻开）
        string lordJson = JsonUtility.ToJson(new CardListWrapper { cards = model.LordCards });
        DeliverLordCardsClientRpc(lordJson);

        // 3. 全员广播：各家手牌数量
        UpdateCardCountsClientRpc(
            model.Player1Cards.Count,
            model.Player2Cards.Count,
            model.Player3Cards.Count,
            model.LordCards.Count);
    }

    [ClientRpc]
    private void DeliverMyCardsClientRpc(string cardsJson, ClientRpcParams clientRpcParams = default)
    {
        NetworkService.Instance.NotifyMyCardsReceived(cardsJson);
    }

    [ClientRpc]
    private void DeliverLordCardsClientRpc(string cardsJson)
    {
        NetworkService.Instance.NotifyLordCardsReceived(cardsJson);
    }

    [ClientRpc]
    private void UpdateCardCountsClientRpc(int p1Count, int p2Count, int p3Count, int lordCount)
    {
        NetworkService.Instance.NotifyCardCountsUpdated(p1Count, p2Count, p3Count, lordCount);
    }

    public void BroadcastBiddingTurn(int playerIndex)
    {
        ChooseLordClientRpc(playerIndex);
    }

    public void BroadcastLordConfirmed(int lordIndex)
    {
        AddLordCardsClientRpc(lordIndex);
    }

    public void BroadcastTurnStart(int playerIndex)
    {
        StartTurnClientRpc(playerIndex);
    }

    public void BroadcastOutCards(string cardsJson)
    {
        ReceiveOutCardClientRpc(cardsJson);
    }

    public void BroadcastPreCardsUpdated(int weight, CardPattern pattern)
    {
        UpdatePreCardsClientRpc(weight, pattern);
    }

    public void BroadcastPreCardsReset()
    {
        ResetPreCardsClientRpc();
    }

    public void BroadcastNotCallLandlord()
    {
        NotCalledLordClientRpc();
    }

    public void BroadcastPassSound()
    {
        PlayPassSoundClientRpc();
    }

    public void BroadcastSoundForPattern(CardPattern pattern, int weight)
    {
        playSoundByPatternClientRpc(pattern, weight);
    }

    public void BroadcastCardCondition(int playerIndex, bool isLord, int remaining)
    {
        CheckGameIsOverClientRpc(playerIndex, isLord, remaining);
    }

    public void SendValidationResult(int result, ulong clientId)
    {
        var clientParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        };
        PlayValidationResultClientRpc(result, clientParams);
    }
    [ClientRpc]
    public void AddLordCardsClientRpc(int lordIndex)
    {
        NetworkService.Instance.NotifyLordConfirmed(lordIndex);
    }

    [ClientRpc]
    public void StartTurnClientRpc(int nextPlayerIndex)
    {
        NetworkService.Instance.NotifyTurnStarted(nextPlayerIndex);
    }
    /// <summary>
    /// 客户端请求轮到下一位出牌，服务端验证后通知 ServerPresenter 推进回合。
    /// </summary>
    /// <param name="callerPlayerIndex"></param>
    /// <param name="rpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void UpdateTurnServerRpc(int callerPlayerIndex, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!ValidateSender(rpcParams, callerPlayerIndex, out ulong sender)) return;
        Debug.Log($"[Server] 更新出牌轮次 sender={sender}, player={callerPlayerIndex}");
        serverPresenter.OnUpdateTurn(callerPlayerIndex);
    }
    /// <summary>
    /// 客户端出牌请求。服务端通过 ClientId 找到玩家编号，交由 ServerPresenter 校验牌型合法性并广播结果。
    /// </summary>
    /// <param name="cardJson"></param>
    [ServerRpc(RequireOwnership = false)]
    public void CheckOutCardServerRpc(string cardJson, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        ulong sender = rpcParams.Receive.SenderClientId;
        if (!clientToPlayerId.TryGetValue(sender, out int playerIndex))
        {
            Debug.LogWarning($"[Server] 未知发送方 {sender}，忽略出牌");
            return;
        }

        // 解析牌组
        var wrapper = JsonUtility.FromJson<CardListWrapper>(cardJson);
        List<Card> outCards = wrapper?.cards ?? new List<Card>();
        serverPresenter.OnPlayCards(playerIndex, outCards, cardJson, sender);
    }
    [ClientRpc]
    public void CheckGameIsOverClientRpc(int playerIndex,bool isLord,int soundIndex)
    {
        if (soundIndex == 0)
            NetworkService.Instance.NotifyGameOver(playerIndex, isLord);
        else
            SoundService.Instance.PlayHandCountReport(soundIndex);
    }
    /// <summary>广播出牌音效，由 SoundService 根据牌型与权重决定具体音效。</summary>
    [ClientRpc]
    public void playSoundByPatternClientRpc(CardPattern pattern,int weight)
    {
        SoundService.Instance.PlaySoundForCardPattern(pattern, weight);
    }
    /// <summary>
    /// 向出牌方客户端发送出牌校验结果（0=非法, 1=合法, 2=王炸）。
    /// </summary>
    /// <param name="result"></param>
    /// <param name="clientRpcParams"></param>
    [ClientRpc]
    public void PlayValidationResultClientRpc(int result, ClientRpcParams clientRpcParams = default)
    {
        NetworkService.Instance.NotifyPlayValidation(result);
    }
    /// <summary>
    /// 广播玩家出牌内容，所有客户端收到后更新出牌区显示。
    /// </summary>
    /// <param name="cardJson"></param>
    [ClientRpc]
    public void ReceiveOutCardClientRpc(string cardJson)
    {
        NetworkService.Instance.NotifyOutCardsReceived(cardJson);
    }
    /// <summary>
    /// 更新上轮出牌的牌型与权重，所有客户端同步。
    /// </summary>
    /// <param name="preWeight"></param>
    /// <param name="prePattern"></param>
    [ClientRpc]
    public void UpdatePreCardsClientRpc(int preWeight, CardPattern prePattern)
    {
        NetworkService.Instance.NotifyPreCardsUpdated(preWeight, prePattern);
    }
    /// <summary>
    /// 客户端过牌请求，服务端验证身份后处理过牌逻辑。
    /// </summary>
    /// <param name="rpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void PassServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        ulong sender = rpcParams.Receive.SenderClientId;
        if (!clientToPlayerId.TryGetValue(sender, out int playerIndex))
        {
            Debug.LogWarning($"[Server] 未知发送方 {sender}，忽略过牌");
            return;
        }

        if (!serverPresenter.OnPass(playerIndex))
        {
            Debug.Log($"[Server] 玩家 {playerIndex} 过牌操作被忽略（重复过牌）");
            return;
        }

        Debug.Log($"[Server] 玩家 {playerIndex} 过牌成功，当前 RefuseCount={serverModel.RefuseCount}");
    }
    [ClientRpc]
    public void PlayPassSoundClientRpc()
    {
        SoundService.Instance.PlayPassCard();
    }
    /// <summary>
    /// 广播清空上轮牌区，所有客户端收到后重置显示。
    /// </summary>
    [ClientRpc]
    public void ResetPreCardsClientRpc()
    {
        NetworkService.Instance.NotifyPreCardsReset();
    }
    /// <summary>
    /// 客户端请求重置上轮牌区（通常由出牌方触发），服务端验证后广播 ResetPreCardsClientRpc。
    /// </summary>
    /// <param name="rpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RequestResetPreCardsServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Debug.Log($"[Server] 客户端 {rpcParams.Receive.SenderClientId} 请求重置上轮牌区");
        serverPresenter.OnRequestResetPreCards();
    }
}
