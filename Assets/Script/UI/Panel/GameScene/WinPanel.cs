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

    private void OnClickReturnButton()
    {
        // 1. 先捕获 NetworkManager 和 GameServer 引用（Shutdown 后 Singleton 会变为 null）
        var nm = NetworkManager.Singleton;
        var gs = GameServer.Instance;
        if (nm != null)
            nm.Shutdown();

        // 2. 清空面板——View 的 OnDisable 中解绑事件
        UIManager.Instance.ClearAllPanels();

        // 3. 重置单例状态
        var ns = NetworkService.Instance;
        if (ns != null) ns.ResetAllEvents();
        var gp = GamePresenter.Instance;
        if (gp != null) gp.ResetForSceneTransition();

        // 4. 销毁 GameServer 的 GameObject（独立于 NetworkManager，必须单独销毁）
        if (gs != null)
            DestroyImmediate(gs.gameObject);

        // 5. 销毁 NetworkManager GameObject
        if (nm != null)
            DestroyImmediate(nm.gameObject);

        // 6. 延迟一帧后加载场景
        SceneLoadHelper.LoadDelayed(SceneManager.GetActiveScene().buildIndex - 1);
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
