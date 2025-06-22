namespace RedGaint.Games.DyeHard.UI
{
    public interface IUIScreen
    {
        void ShowScreen(UIScreenContext context = null);
        void HideScreen();
    }
}