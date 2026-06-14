/// <summary>Presenter 打开界面时依赖的抽象，便于测试或替换 UIManager。</summary>
public interface IUiNavigator
{
    BasePanel OpenPanel(string panelName);
    BasePanel GetPanel(string panelName);
}
