using System.Windows;

namespace Bsw.Coworking.Agent.Config.Utilities.Services
{
    public interface IFetchCurrentWindow
    {
        Window CurrentWindow { get; }
    }
}