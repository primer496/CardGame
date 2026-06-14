/// <summary>默认实现：外观代理现有 UIManager。</summary>
public sealed class UIManagerUiNavigator : IUiNavigator
{
    public BasePanel OpenPanel(string panelName)
    {
        return UIManager.Instance == null ? null : UIManager.Instance.OpenPanel(panelName);
    }

    public BasePanel GetPanel(string panelName)
    {
        return UIManager.Instance == null ? null : UIManager.Instance.GetPanel(panelName);
    }
}
