using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StartPanel : BasePanel, IStartView
{
    public event Action PanelOpened;
    public event Action StartClicked;
    public event Action RegisterClicked;

    private Transform UIStartButton;
    private Transform UIRegisterButton;

    override protected void Awake()
    {
        UIStartButton = transform.Find("StartButton");
        UIRegisterButton = transform.Find("RegisterButton");

        UIStartButton.GetComponent<Button>().onClick.AddListener(StarButtonOnclick);
        UIRegisterButton.GetComponent<Button>().onClick.AddListener(RegisterButtonOnclick);
        StartScenePresenter.Ensure();
    }
    private void Start()
    {
        PanelOpened?.Invoke();
    }
    private void StarButtonOnclick()
    {
        StartClicked?.Invoke();
    }
    private void RegisterButtonOnclick()
    {
        RegisterClicked?.Invoke();
    }
}
