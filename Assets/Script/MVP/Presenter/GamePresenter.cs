using UnityEngine;

public partial class GamePresenter : MonoBehaviour
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

    /// <summary>删除场景内旧 GameManager 后仍必须创建 MVP 栈，否则 GamePanel 事件无人订阅。</summary>
    public static void EnsureClientStack()
    {
        _ = NetworkService.Instance;
        _ = SoundService.Instance;
        _ = Instance;
    }

    [SerializeField] private GamePanel gamePanelViewRef;
    [SerializeField] private PlayerCtrl playerHandViewRef;
    [SerializeField] private LeftPlayerCtrl leftPlayerViewRef;
    [SerializeField] private RightPlayerCtrl rightPlayerViewRef;

    private IOpponentHandView leftOpponentView;
    private IOpponentHandView rightOpponentView;
    [SerializeField] private OutCardAreaCtrl outCardAreaViewRef;
    [SerializeField] private LoadCards lordCardsViewRef;
    [SerializeField] private WinPanel winViewRef;

    private IGamePanelView gameView;
    private IPlayerHandView playerHandView;
    private IOutCardAreaView outCardAreaView;
    private ILordCardsView lordCardsView;
    private IWinView winView;

    private GameModel model;
    private INetworkService networkService;
    private ISoundService soundService;
    private IUiNavigator uiNavigator;
    private BiddingPresenter biddingPresenter;
    private PlayPresenter playPresenter;
    private ResultPresenter resultPresenter;
    private bool isViewEventsBound;
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
        uiNavigator = new UIManagerUiNavigator();
        model = new GameModel();
        networkService.ApplyCachedPlayerIdToModel(model);

        ResolveViews();
    }

    private void OnEnable()
    {
        BindViewEvents();
        BindNetworkEvents();
    }

    private void OnDisable()
    {
        UnbindViewEvents();
        UnbindNetworkEvents();
    }

    private void ResolveViews()
    {
        if (gamePanelViewRef == null) gamePanelViewRef = FindObjectOfType<GamePanel>();
        if (playerHandViewRef == null) playerHandViewRef = FindObjectOfType<PlayerCtrl>();
        if (leftPlayerViewRef == null) leftPlayerViewRef = FindObjectOfType<LeftPlayerCtrl>();
        if (rightPlayerViewRef == null) rightPlayerViewRef = FindObjectOfType<RightPlayerCtrl>();
        if (outCardAreaViewRef == null) outCardAreaViewRef = FindObjectOfType<OutCardAreaCtrl>();
        if (lordCardsViewRef == null) lordCardsViewRef = FindObjectOfType<LoadCards>();
        if (winViewRef == null) winViewRef = FindObjectOfType<WinPanel>();

        gameView = gamePanelViewRef;
        playerHandView = playerHandViewRef;
        leftOpponentView = leftPlayerViewRef;
        rightOpponentView = rightPlayerViewRef;
        outCardAreaView = outCardAreaViewRef;
        lordCardsView = lordCardsViewRef;
        winView = winViewRef;

        if (model != null && networkService != null && soundService != null && uiNavigator != null)
            BuildChildPresenters();
    }

    private void BuildChildPresenters()
    {
        biddingPresenter = new BiddingPresenter(model, networkService, soundService, gameView, lordCardsView);
        playPresenter = new PlayPresenter(
            model,
            networkService,
            playerHandView,
            leftOpponentView,
            rightOpponentView,
            gameView,
            outCardAreaView,
            lordCardsView);
        resultPresenter = new ResultPresenter(model, soundService, uiNavigator, ResolveWinViewFallback);
    }

    private IWinView ResolveWinViewFallback()
    {
        ResolveViews();
        return winView;
    }

    private void BindViewEvents()
    {
        if (gameView == null || isViewEventsBound) return;
        gameView.GameViewReady += OnGameViewReady;
        gameView.PrepareClicked += OnLocalPrepare;
        gameView.OutClicked += OnLocalPlayCards;
        gameView.PassClicked += OnLocalPass;
        gameView.AcceptLordClicked += OnLocalAcceptLord;
        gameView.RefuseLordClicked += OnLocalRefuseLord;
        isViewEventsBound = true;
    }

    private void UnbindViewEvents()
    {
        if (gameView == null || !isViewEventsBound) return;
        gameView.GameViewReady -= OnGameViewReady;
        gameView.PrepareClicked -= OnLocalPrepare;
        gameView.OutClicked -= OnLocalPlayCards;
        gameView.PassClicked -= OnLocalPass;
        gameView.AcceptLordClicked -= OnLocalAcceptLord;
        gameView.RefuseLordClicked -= OnLocalRefuseLord;
        isViewEventsBound = false;
    }

    private void BindNetworkEvents()
    {
        if (networkService == null || isNetworkEventsBound) return;

        networkService.OnCardsDelivered += OnCardsReceived;
        networkService.OnBiddingTurn += OnBiddingTurn;
        networkService.OnPlayerIdAssigned += OnPlayerIdAssigned;
        networkService.OnLordConfirmed += OnLordConfirmed;
        networkService.OnTurnStarted += OnTurnStart;
        networkService.OnOutCardsReceived += OnOutCardsReceived;
        networkService.OnPlayValidation += OnPlayValidation;
        networkService.OnGameOver += OnGameOver;
        networkService.OnPreCardsUpdated += OnPreCardsUpdated;
        networkService.OnPreCardsReset += OnPreCardsReset;

        networkService.ApplyCachedPlayerIdToModel(model);
        isNetworkEventsBound = true;
    }

    private void UnbindNetworkEvents()
    {
        if (networkService == null || !isNetworkEventsBound) return;

        networkService.OnCardsDelivered -= OnCardsReceived;
        networkService.OnBiddingTurn -= OnBiddingTurn;
        networkService.OnPlayerIdAssigned -= OnPlayerIdAssigned;
        networkService.OnLordConfirmed -= OnLordConfirmed;
        networkService.OnTurnStarted -= OnTurnStart;
        networkService.OnOutCardsReceived -= OnOutCardsReceived;
        networkService.OnPlayValidation -= OnPlayValidation;
        networkService.OnGameOver -= OnGameOver;
        networkService.OnPreCardsUpdated -= OnPreCardsUpdated;
        networkService.OnPreCardsReset -= OnPreCardsReset;
        isNetworkEventsBound = false;
    }

    private void OnGameViewReady()
    {
        soundService.PlayGameBgm();
    }
}
