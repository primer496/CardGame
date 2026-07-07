using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WinPanel : BasePanel
{
    private Transform UIWinText;
    private Transform UIRewardText;
    private Transform UIReturnButton;

    protected override void Awake()
    {
        ResolveRefs();

        if (UIReturnButton != null)
        {
            var btn = UIReturnButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnClickReturnButton);
            }
        }
    }

    private void OnEnable()
    {
        var ns = NetworkService.Instance;
        if (ns != null) ns.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        var ns = NetworkService.Instance;
        if (ns != null) ns.OnGameOver -= OnGameOver;
    }

    private void OnGameOver(int playerIndex, bool isLord)
    {
        var model = GamePresenter.Instance?.Model;
        if (model == null) return;

        ShowWinResult(isLord);
        if (playerIndex == model.LocalPlayerIndex)
            SoundService.Instance?.PlayWinBgm();
        else
            SoundService.Instance?.PlayLoseBgm();
    }

    private void OnClickReturnButton()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        UIManager.Instance.ClearAllPanels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ShowWinResult(bool isLord)
    {
        ResolveRefs();
        if (UIWinText == null)
        {
            Debug.LogWarning("WinPanel 缺少 WinText 节点，无法显示结算文本。");
            return;
        }

        string resultText = isLord ? "地主胜利！" : "农民胜利！";
        Text text = UIWinText.GetComponent<Text>();
        if (text != null)
        {
            text.text = resultText;
            return;
        }

        TextMeshProUGUI tmpText = UIWinText.GetComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = resultText;
            return;
        }

        Debug.LogWarning("WinPanel 的 WinText 未挂载 Text 或 TextMeshProUGUI 组件。");
    }

    private void ResolveRefs()
    {
        if (UIWinText == null) UIWinText = transform.Find("WinText");
        if (UIRewardText == null) UIRewardText = transform.Find("RewardText");
        if (UIReturnButton == null) UIReturnButton = transform.Find("ReturnButton");
    }
}
