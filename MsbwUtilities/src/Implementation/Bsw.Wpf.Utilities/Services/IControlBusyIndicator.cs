namespace Bsw.Coworking.Agent.Config.Utilities.Services
{
    public interface IControlBusyIndicator
    {
        void Show(string text = "Loading...");
        void Hide();
    }
}