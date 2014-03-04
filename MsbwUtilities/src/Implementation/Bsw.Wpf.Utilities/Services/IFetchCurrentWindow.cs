using System.Windows;

namespace Bsw.Wpf.Utilities.Services
{
    public interface IFetchCurrentWindow
    {
        Window CurrentWindow { get; }
    }
}