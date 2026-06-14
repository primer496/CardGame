using System;

public interface IWinView
{
    event Action ReturnClicked;
    void ShowWinResult(bool isLord);
}
