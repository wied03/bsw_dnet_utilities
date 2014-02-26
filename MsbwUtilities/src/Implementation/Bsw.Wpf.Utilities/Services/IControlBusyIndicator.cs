using System;

namespace Bsw.Wpf.Utilities.Services
{
    public interface IControlBusyIndicator : IDisposable
    {
        void Show(string text = "Loading...");
    }
}