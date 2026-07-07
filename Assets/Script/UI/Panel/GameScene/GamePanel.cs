using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePanel : BasePanel
{
    private Transform UIHasPrepareText;
    private Transform UIDialogueButton;
    private Transform UIPrepareButton;
    private Transform UIPassButton;
    private Transform UIOutButton;
    private Transform UIGetLoadButton;
    private Transform UIRefuseLoadButton;
    private Transform UICountTextr;
    private Transform UICountTextl;

    [SerializeField] private PlayerCtrl playerCtrl;

    protected override void Awake()
    {
        UIHasPrepareText = transform.Find("GameButton/Prepare/HasPrepare");
        UIDialogueButton = transform.Find("Bottom/DialogueButton");
        UIPrepareButton = transform.Find("GameButton/Prepare/PrepareButton");
        UIOutButton = transform.Find("GameButton/OutButton");
        UIPassButton = transform.Find("GameButton/PassButton");
        UIGetLoadButton=transform.Find("GameButton/GetLoadButton");
        UIRefuseLoadButton=transform.Find("GameButton/RefuseLoadButton");
        UICountTextr = transform.Find("Enemyr/CountText");
        UICountTextl = transform.Find("Enemyl/CountText");

        UIDialogueButton.GetComponent<Button>().onClick.AddListener(OnClickDialogueButton);
        UIPrepareButton.GetComponent<Button>().onClick.AddListener(OnClickPrepareButton);
        UIOutButton.GetComponent<Button>().onClick.AddListener(OnClickOutButton);
        UIPassButton.GetComponent<Button>().onClick.AddListener(OnClickPassButton);
        UIGetLoadButton.GetComponent<Button>().onClick.AddListener(GetLoadCard);
        UIRefuseLoadButton.GetComponent<Button>().onClick.AddListener(RefuseLoadCard);

        UIHasPrepareText.gameObject.SetActive(false);
        UIPassButton.gameObject.SetActive(false);
        UIOutButton.gameObject.SetActive(false);
        UIGetLoadButton.gameObject.SetActive(false);
        UIRefuseLoadButton.gameObject.SetActive(false);

        GamePresenter.EnsureClientStack();
    }

    private void OnEnable()
    {
        var ns = NetworkService.Instance;
        if (ns != null)
        {
            ns.OnBiddingTurn += OnBiddingTurn;
            ns.OnTurnStarted += OnTurnStart;
            ns.OnGameOver += OnGameOver;
        }

        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated += OnLocalCardsUpdated;
    }

    private void OnDisable()
    {
        var ns = NetworkService.Instance;
        if (ns != null)
        {
            ns.OnBiddingTurn -= OnBiddingTurn;
            ns.OnTurnStarted -= OnTurnStart;
            ns.OnGameOver -= OnGameOver;
        }

        var gp = GamePresenter.Instance;
        if (gp != null) gp.OnLocalCardsUpdated -= OnLocalCardsUpdated;
    }

    private void Start()
    {
        SoundService.Instance?.PlayGameBgm();
    }

    // ── 按钮回调（直接调用 NetworkService） ──

    private void OnClickDialogueButton()
    {
        Debug.Log("展开快捷对话");
    }

    private void OnClickPrepareButton()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        NetworkService.Instance.ApplyCachedPlayerIdToModel(model);
        if (model.LocalPlayerIndex == 0)
        {
            Debug.LogWarning("[Client] 尚未分配到玩家编号，请稍后再点准备。");
            return;
        }

        Debug.Log($"[Client] 发送准备，playerId={model.LocalPlayerIndex}");
        NetworkService.Instance.PreparePlayer(model.LocalPlayerIndex);
        ShowPrepareButton(false);
    }

    private void OnClickOutButton()
    {
        if (playerCtrl == null)
        {
            playerCtrl = FindObjectOfType<PlayerCtrl>();
            if (playerCtrl == null) return;
        }

        var selected = playerCtrl.GetSelectedCards();
        string json = JsonUtility.ToJson(new CardListWrapper { cards = selected });
        NetworkService.Instance.SendPlayCards(json);
    }

    private void OnClickPassButton()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        NetworkService.Instance.SendPass();
        NetworkService.Instance.RequestNextTurn(model.LocalPlayerIndex);
    }

    private void GetLoadCard()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        NetworkService.Instance.SendAcceptLord(model.LocalPlayerIndex);
        UIGetLoadButton.gameObject.SetActive(false);
        UIRefuseLoadButton.gameObject.SetActive(false);
    }

    private void RefuseLoadCard()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        NetworkService.Instance.SendRefuseLord(model.LocalPlayerIndex);
        UIRefuseLoadButton.gameObject.SetActive(false);
        UIGetLoadButton.gameObject.SetActive(false);
    }

    // ── 网络事件回调 ──

    private void OnBiddingTurn(int playerIndex)
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;
        ShowBiddingUI(playerIndex == model.LocalPlayerIndex);
    }

    private void OnTurnStart(int playerIndex)
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;
        model.StartTurn(playerIndex);
        SetTurnUI(playerIndex == model.LocalPlayerIndex);
    }

    private void OnLocalCardsUpdated()
    {
        RefreshEnemyCardCount();
    }

    private void OnGameOver(int playerIndex, bool isLord)
    {
        // WinPanel 负责显示，GamePanel 可在此隐藏游戏 UI
    }

    // ── UI 刷新 ──

    public void ShowBiddingUI(bool visible)
    {
        UIGetLoadButton.gameObject.SetActive(visible);
        UIRefuseLoadButton.gameObject.SetActive(visible);
    }

    public void ShowPlayButtons(bool visible)
    {
        UIPassButton.gameObject.SetActive(visible);
        UIOutButton.gameObject.SetActive(visible);
    }

    public void ShowPrepareButton(bool visible)
    {
        UIPrepareButton.gameObject.SetActive(visible);
    }

    public void SetTurnUI(bool isLocalTurn)
    {
        ShowPlayButtons(isLocalTurn);
    }

    private void RefreshEnemyCardCount()
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        if (UICountTextr == null) UICountTextr = transform.Find("Enemyr/CountText");
        if (UICountTextl == null) UICountTextl = transform.Find("Enemyl/CountText");

        int leftCount = model.GetPlayerCards(model.LeftPlayerIndex).Count;
        int rightCount = model.GetPlayerCards(model.RightPlayerIndex).Count;

        var rightText = UICountTextr != null ? UICountTextr.GetComponent<TextMeshProUGUI>() : null;
        var leftText = UICountTextl != null ? UICountTextl.GetComponent<TextMeshProUGUI>() : null;

        if (rightText != null) rightText.text = rightCount.ToString();
        if (leftText != null) leftText.text = leftCount.ToString();
    }
}
