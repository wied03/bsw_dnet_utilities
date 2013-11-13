using Microsoft.Practices.Prism.ViewModel;
using MsBw.MsBwUtility.JetBrains.Annotations;

namespace Bsw.Coworking.Agent.Config.Utilities.ViewModels
{
    // code credited to http://www.codeproject.com/Articles/97564/Attributes-based-Validation-in-a-WPF-MVVM-Applicat

    /// <summary>
    ///     Base class for all ViewModel classes in the application. Provides support for
    ///     property changes notification.
    /// </summary>
    [UsedImplicitly]
    public abstract class ViewModelBase : NotificationObject
    {
    }
}