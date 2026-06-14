using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    private Transform UIStartButton;
    private Transform UISetButton;

    protected override void Awake()
    {
        UIStartButton = transform.Find("StartButton");
        UISetButton = transform.Find("SetButton");

        UIStartButton.GetComponent<Button>().onClick.AddListener(OnClickStartButton);
        UISetButton.GetComponent<Button>().onClick.AddListener(OnClickSetButton);
    }
    private void OnClickStartButton()
    {
        UIManager.Instance.OpenPanel(UIConst.EnterPanel);
    }
    private void OnClickSetButton()
    {
        UIManager.Instance.OpenPanel(UIConst.SettingPanel);
    }
}
