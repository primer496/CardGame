using System;
using UnityEngine;

public class NetworkService : MonoBehaviour, INetworkService
{
    private static NetworkService _instance;
    public static NetworkService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkService>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("NetworkService");
                    _instance = go.AddComponent<NetworkService>();
                }
            }

            return _instance;
        }
    }

    public event Action<string, string, string, string> OnCardsDelivered;
    public event Action<int> OnBiddingTurn;
    public event Action<int> OnPlayerIdAssigned;
    public event Action<int> OnLordConfirmed;
    public event Action<int> OnTurnStarted;
    public event Action<string> OnOutCardsReceived;
    public event Action<int> OnPlayValidation;
    public event Action<int, bool> OnGameOver;
    public event Action<int, CardPattern> OnPreCardsUpdated;
    public event Action OnPreCardsReset;

    /// <summary>服务端分配的玩家编号(1..3)。在 GamePresenter 晚于 ClientRpc 创建时用于补填 LocalPlayerIndex。</summary>
    private int _cachedAssignedPlayerIndex;

    public int CachedAssignedPlayerIndex => _cachedAssignedPlayerIndex;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PreparePlayer(int localId)
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.CheckStartedServerRpc(localId);
    }

    public void SendPlayCards(string cardsJson)
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.CheckOutCardServerRpc(cardsJson);
    }

    public void SendPass()
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.PassServerRpc();
    }

    public void SendAcceptLord(int localId)
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.SetLordIndexServerRpc(localId);
    }

    public void SendRefuseLord(int currentId)
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.ChooseLordServerRpc(currentId);
    }

    public void RequestNextTurn(int localId)
    {
        if (GameServer.Instance == null) return;
        GameServer.Instance.UpdateTurnServerRpc(localId);
    }

    public void NotifyCardsDelivered(string json1, string json2, string json3, string lordJson)
    {
        OnCardsDelivered?.Invoke(json1, json2, json3, lordJson);
    }

    public void NotifyBiddingTurn(int playerIndex)
    {
        OnBiddingTurn?.Invoke(playerIndex);
    }

    public void NotifyPlayerIdAssigned(int playerIndex)
    {
        if (playerIndex >= 1 && playerIndex <= 3)
            _cachedAssignedPlayerIndex = playerIndex;

        OnPlayerIdAssigned?.Invoke(playerIndex);
    }

    public void ApplyCachedPlayerIdToModel(GameModel targetModel)
    {
        if (targetModel == null) return;
        if (_cachedAssignedPlayerIndex < 1 || _cachedAssignedPlayerIndex > 3) return;
        if (targetModel.LocalPlayerIndex != 0) return;

        targetModel.LocalPlayerIndex = _cachedAssignedPlayerIndex;
    }

    public void NotifyLordConfirmed(int lordIndex)
    {
        OnLordConfirmed?.Invoke(lordIndex);
    }

    public void NotifyTurnStarted(int playerIndex)
    {
        OnTurnStarted?.Invoke(playerIndex);
    }

    public void NotifyOutCardsReceived(string cardsJson)
    {
        OnOutCardsReceived?.Invoke(cardsJson);
    }

    public void NotifyPlayValidation(int result)
    {
        OnPlayValidation?.Invoke(result);
    }

    public void NotifyGameOver(int playerIndex, bool isLord)
    {
        OnGameOver?.Invoke(playerIndex, isLord);
    }

    public void NotifyPreCardsUpdated(int preWeight, CardPattern prePattern)
    {
        OnPreCardsUpdated?.Invoke(preWeight, prePattern);
    }

    public void NotifyPreCardsReset()
    {
        OnPreCardsReset?.Invoke();
    }
}
