using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPresenter
{
    private readonly GameModel model;
    private readonly ISoundService soundService;
    private readonly IUiNavigator uiNavigator;
    private readonly Func<IWinView> fallbackWinViewResolver;
    private IWinView currentWinView;

    public ResultPresenter(
        GameModel model,
        ISoundService soundService,
        IUiNavigator uiNavigator,
        Func<IWinView> fallbackWinViewResolver)
    {
        this.model = model;
        this.soundService = soundService;
        this.uiNavigator = uiNavigator;
        this.fallbackWinViewResolver = fallbackWinViewResolver;
    }

    public void OnGameOver(int playerIndex, bool isLord)
    {
        IWinView activeWinView = null;

        try
        {
            BasePanel openedPanel = uiNavigator.OpenPanel(UIConst.WinPanel);
            BasePanel currentPanel = openedPanel ?? uiNavigator.GetPanel(UIConst.WinPanel);
            activeWinView = currentPanel as IWinView;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"打开胜利界面失败: {ex}");
        }

        if (activeWinView == null)
            activeWinView = fallbackWinViewResolver?.Invoke();

        BindWinViewEvents(activeWinView);
        activeWinView?.ShowWinResult(isLord);
        if (playerIndex == model.LocalPlayerIndex)
            soundService.PlayWinBgm();
        else
            soundService.PlayLoseBgm();
    }

    private void BindWinViewEvents(IWinView target)
    {
        if (currentWinView != null)
            currentWinView.ReturnClicked -= OnReturnClicked;

        currentWinView = target;
        if (currentWinView != null)
            currentWinView.ReturnClicked += OnReturnClicked;
    }

    private void OnReturnClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
