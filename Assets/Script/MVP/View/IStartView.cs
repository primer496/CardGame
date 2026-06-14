using System;

public interface IStartView
{
    event Action PanelOpened;
    event Action StartClicked;
    event Action RegisterClicked;
}
