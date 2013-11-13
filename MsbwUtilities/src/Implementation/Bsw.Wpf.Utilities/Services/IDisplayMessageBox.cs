using System;

namespace Bsw.Coworking.Agent.Config.Utilities.Services
{
    public enum DialogType
    {
        Error,
        Information,
        Warning,
        Success
    }

    public interface IDisplayMessageBox
    {
        void Display(string messageText,
                     DialogType dialogType);

        void DisplayError(string prompt,
                          Exception exception);
    }
}