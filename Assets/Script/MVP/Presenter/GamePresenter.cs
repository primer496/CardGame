using System;
using UnityEngine;

/// <summary>
/// 最简生命周期管理：持有 GameModel。
/// 作为网络发牌相关事件的唯一订阅者，先更新 Model 再通过 OnLocalCardsUpdated 通知 View，
/// 保证 View 读取到的 Model 始终是最新的。
/// </summary>
public class GamePresenter : MonoBehaviour
{
    private static GamePresenter _instance;
    public static GamePresenter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GamePresenter>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GamePresenter");
                    _instance = go.AddComponent<GamePresenter>();
                }
            }
            return _instance;
        }
    }

    /// <summary>确保 NetworkService / SoundService / GamePresenter 单例已创建。</summary>
    public static void EnsureClientStack()
    {
        _ = NetworkService.Instance;
        _ = SoundService.Instance;
        _ = Instance;
    }

    public GameModel Model { get; private set; }

    /// <summary>本地事件：Model 中的手牌数据已更新，View 应自行刷新。</summary>
    public event Action OnLocalCardsUpdated;

    private INetworkService networkService;
    private ISoundService soundService;
    private bool isNetworkEventsBound;

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
            return;
        }

        networkService = NetworkService.Instance;
        soundService = SoundService.Instance;
        Model = new GameModel();
        networkService.ApplyCachedPlayerIdToModel(Model);
    }

    private void OnEnable()
    {
        BindNetworkEvents();
    }

    private void OnDisable()
    {
        UnbindNetworkEvents();
    }

    private void BindNetworkEvents()
    {
        if (networkService == null || isNetworkEventsBound) return;
        networkService.OnPlayerIdAssigned += OnPlayerIdAssigned;
        networkService.OnMyCardsReceived += OnMyCardsReceived;
        networkService.OnLordCardsReceived += OnLordCardsReceived;
        networkService.OnCardCountsUpdated += OnCardCountsUpdated;
        networkService.OnPreCardsUpdated += OnPreCardsUpdated;
        networkService.OnPreCardsReset += OnPreCardsReset;
        isNetworkEventsBound = true;
    }

    private void UnbindNetworkEvents()
    {
        if (networkService == null || !isNetworkEventsBound) return;
        networkService.OnPlayerIdAssigned -= OnPlayerIdAssigned;
        networkService.OnMyCardsReceived -= OnMyCardsReceived;
        networkService.OnLordCardsReceived -= OnLordCardsReceived;
        networkService.OnCardCountsUpdated -= OnCardCountsUpdated;
        networkService.OnPreCardsUpdated -= OnPreCardsUpdated;
        networkService.OnPreCardsReset -= OnPreCardsReset;
        isNetworkEventsBound = false;
    }

    private void OnPlayerIdAssigned(int playerIndex)
    {
        if (Model.LocalPlayerIndex != 0) return;
        Model.LocalPlayerIndex = playerIndex;
        Debug.Log($"[Client] 收到玩家编号分配：{Model.LocalPlayerIndex}");
    }

    /// <summary>定向：仅收到自己的手牌，反序列化后存入 Model。</summary>
    private void OnMyCardsReceived(string cardsJson)
    {
        var wrapper = JsonUtility.FromJson<CardListWrapper>(cardsJson);
        Model.ReplaceMyCards(wrapper.cards);
        TryFireLocalCardsUpdated();
    }

    /// <summary>广播：全员收到底牌。</summary>
    private void OnLordCardsReceived(string cardsJson)
    {
        var wrapper = JsonUtility.FromJson<CardListWrapper>(cardsJson);
        Model.ReplaceLordCards(wrapper.cards);
        TryFireLocalCardsUpdated();
    }

    /// <summary>广播：全员收到各玩家手牌数。</summary>
    private void OnCardCountsUpdated(int p1, int p2, int p3, int lord)
    {
        Model.UpdateCardCounts(p1, p2, p3, lord);
        TryFireLocalCardsUpdated();
    }

    /// <summary>
    /// 三个定向事件任一到达即尝试通知 View 刷新。
    /// 在全部数据到齐前（EarlyReturn），View 可能被多次触发，但每次读取的都是最新局部数据，无副作用。
    /// </summary>
    private void TryFireLocalCardsUpdated()
    {
        OnLocalCardsUpdated?.Invoke();
    }

    private void OnPreCardsUpdated(int preWeight, CardPattern prePattern)
    {
        Model.PreWeight = preWeight;
        Model.PrePattern = prePattern;
    }

    private void OnPreCardsReset()
    {
        Model.ResetPreCards();
    }
}
