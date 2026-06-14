public class BiddingPresenter
{
    private readonly GameModel model;
    private readonly INetworkService networkService;
    private readonly ISoundService soundService;
    private readonly IGamePanelView gameView;
    private readonly ILordCardsView lordCardsView;

    public BiddingPresenter(
        GameModel model,
        INetworkService networkService,
        ISoundService soundService,
        IGamePanelView gameView,
        ILordCardsView lordCardsView)
    {
        this.model = model;
        this.networkService = networkService;
        this.soundService = soundService;
        this.gameView = gameView;
        this.lordCardsView = lordCardsView;
    }

    public void OnBiddingTurn(int playerIndex)
    {
        bool isLocal = playerIndex == model.LocalPlayerIndex;
        gameView?.ShowBiddingUI(isLocal);
    }

    public void OnAcceptLord()
    {
        networkService.SendAcceptLord(model.LocalPlayerIndex);
    }

    public void OnRefuseLord()
    {
        networkService.SendRefuseLord(model.LocalPlayerIndex);
    }

    public void OnLordConfirmed(int lordIndex)
    {
        soundService.PlayCallLandlord();
        lordCardsView?.ShowLordCards();
    }
}
