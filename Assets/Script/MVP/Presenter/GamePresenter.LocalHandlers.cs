using UnityEngine;

/// <summary>View 按钮事件：转调 NetworkService（与网络回调文件对称）。</summary>
public partial class GamePresenter
{
    public void OnLocalAcceptLord()
    {
        biddingPresenter?.OnAcceptLord();
    }

    public void OnLocalRefuseLord()
    {
        biddingPresenter?.OnRefuseLord();
    }

    public void OnLocalPrepare()
    {
        networkService.ApplyCachedPlayerIdToModel(model);
        if (model.LocalPlayerIndex == 0)
        {
            Debug.LogWarning("[Client] 尚未分配到玩家编号（等待服务端 AssignPlayerId），请稍后再点准备。");
            return;
        }

        Debug.Log($"[Client] 发送准备 CheckStartedServerRpc，playerId={model.LocalPlayerIndex}");
        networkService.PreparePlayer(model.LocalPlayerIndex);
        gameView?.ShowPrepareButton(false);
    }

    public void OnLocalPlayCards()
    {
        playPresenter?.OnLocalPlayCards();
    }

    public void OnLocalPass()
    {
        playPresenter?.OnLocalPass();
    }
}
