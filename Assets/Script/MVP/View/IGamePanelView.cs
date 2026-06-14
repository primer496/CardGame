using System;

public interface IGamePanelView
{
    /// <summary>视图完成首帧初始化（用于 Presenter 播放场景 BGM 等，View 不直接调用 Service）。</summary>
    event Action GameViewReady;
    event Action PrepareClicked;
    event Action OutClicked;
    event Action PassClicked;
    event Action AcceptLordClicked;
    event Action RefuseLordClicked;

    void ShowBiddingUI(bool visible);
    void ShowPlayButtons(bool visible);
    void ShowPrepareButton(bool visible);
    void SetTurnUI(bool isLocalTurn);
    void RefreshPlayerCardCount(int leftCount, int rightCount);
}
