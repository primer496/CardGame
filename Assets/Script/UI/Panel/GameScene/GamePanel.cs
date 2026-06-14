using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GamePanel : BasePanel, IGamePanelView
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

    public event Action GameViewReady;
    public event Action PrepareClicked;
    public event Action OutClicked;
    public event Action PassClicked;
    public event Action AcceptLordClicked;
    public event Action RefuseLordClicked;

    protected override void Awake()
    {
        UIHasPrepareText = transform.Find("GameButton/Prepare/HasPrepare");
        UIDialogueButton = transform.Find("Bottom/DialogueButton");
        UIPrepareButton = transform.Find("GameButton/Prepare/PrepareButton");
        UIOutButton = transform.Find("GameButton/OutButton");
        UIPassButton = transform.Find("GameButton/PassButton");
        UIGetLoadButton=transform.Find("GameButton/GetLoadButton");
        UIRefuseLoadButton=transform.Find("GameButton/RefuseLoadButton");
        // 缓存敌人计数文本引用，避免每次 Update 查找
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
    private void Start()
    {
        GameViewReady?.Invoke();
    }

    private void OnClickDialogueButton()
    {
        Debug.Log("展开快捷对话");
    }
    private void OnClickPrepareButton()
    {
        PrepareClicked?.Invoke();
    }
    private void OnClickOutButton()
    {
        OutClicked?.Invoke();
    }
    private void OnClickPassButton()
    {
        PassClicked?.Invoke();
    }
    private void GetLoadCard()
    {
        AcceptLordClicked?.Invoke();
        UIGetLoadButton.gameObject.SetActive(false);
        UIRefuseLoadButton.gameObject.SetActive(false);
    }
    private void RefuseLoadCard()
    {
        RefuseLordClicked?.Invoke();
        UIRefuseLoadButton.gameObject.SetActive(false);
        UIGetLoadButton.gameObject.SetActive(false);
    }

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

    public void RefreshPlayerCardCount(int leftCount, int rightCount)
    {
        if (UICountTextr == null) UICountTextr = transform.Find("Enemyr/CountText");
        if (UICountTextl == null) UICountTextl = transform.Find("Enemyl/CountText");

        var rightText = UICountTextr != null ? UICountTextr.GetComponent<TextMeshProUGUI>() : null;
        var leftText = UICountTextl != null ? UICountTextl.GetComponent<TextMeshProUGUI>() : null;

        if (rightText != null) rightText.text = rightCount.ToString();
        if (leftText != null) leftText.text = leftCount.ToString();
    }
}
