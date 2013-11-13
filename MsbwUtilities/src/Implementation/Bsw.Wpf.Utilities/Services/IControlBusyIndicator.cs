namespace Bsw.Wpf.Utilities.Services
{
    public interface IControlBusyIndicator
    {
        void Show(string text = "Loading...");
        void Hide();
    }
}