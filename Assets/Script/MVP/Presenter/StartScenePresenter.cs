using UnityEngine;

/// <summary>Start 场景的轻量 Presenter：接管 StartPanel 的 BGM 播放。</summary>
public class StartScenePresenter : MonoBehaviour
{
    private static StartScenePresenter _instance;
    [SerializeField] private StartPanel startPanelViewRef;
    private IStartView startView;
    private ISoundService soundService;
    private IUiNavigator uiNavigator;
    private bool isStartViewEventsBound;

    public static void Ensure()
    {
        _ = Instance;
    }

    private static StartScenePresenter Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<StartScenePresenter>();
            if (_instance == null)
            {
                GameObject go = new GameObject("StartScenePresenter");
                _instance = go.AddComponent<StartScenePresenter>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 强制横屏（1920×1080 设计基准），避免部分设备忽略 Build Settings 的朝向配置
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        soundService = SoundService.Instance;
        uiNavigator = new UIManagerUiNavigator();
        ResolveStartView();
        BindStartViewEvents();
    }

    private void OnEnable()
    {
        ResolveStartView();
        BindStartViewEvents();
    }

    private void OnDisable()
    {
        UnbindStartViewEvents();
    }

    private void OnDestroy()
    {
        UnbindStartViewEvents();
    }

    private void ResolveStartView()
    {
        if (startPanelViewRef == null) startPanelViewRef = FindObjectOfType<StartPanel>();
        startView = startPanelViewRef;
    }

    private void BindStartViewEvents()
    {
        if (startView == null || isStartViewEventsBound) return;
        startView.PanelOpened += OnPanelOpened;
        startView.StartClicked += OnStartClicked;
        startView.RegisterClicked += OnRegisterClicked;
        isStartViewEventsBound = true;
    }

    private void UnbindStartViewEvents()
    {
        if (startView == null || !isStartViewEventsBound) return;
        startView.PanelOpened -= OnPanelOpened;
        startView.StartClicked -= OnStartClicked;
        startView.RegisterClicked -= OnRegisterClicked;
        isStartViewEventsBound = false;
    }

    private void OnPanelOpened()
    {
        soundService.PlayStartBgm();
    }

    private void OnStartClicked()
    {
        uiNavigator.OpenPanel(UIConst.LoadPanel);
    }

    private void OnRegisterClicked()
    {
        uiNavigator.OpenPanel(UIConst.RegisterPanel);
    }
}
