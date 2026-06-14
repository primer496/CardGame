using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : BasePanel, IWinView
{
    public event System.Action ReturnClicked;

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
        ReturnClicked?.Invoke();
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
