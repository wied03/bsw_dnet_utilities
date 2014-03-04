using System;

namespace Bsw.Wpf.Utilities.Services
{
    public interface IControlBusyIndicator : IDisposable
    {
        IControlBusyIndicator Show(string text = "Loading...");
    }
}