using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private Transform UIExitButton;

    protected override void Awake()
    {
        UIExitButton = transform.Find("ExitButton");

        UIExitButton.GetComponent<Button>().onClick.AddListener(OnClickExit);
    }
    private void OnClickExit()
    {
        UIManager.Instance.ClosePanel(UIConst.SettingPanel);
    }
}
