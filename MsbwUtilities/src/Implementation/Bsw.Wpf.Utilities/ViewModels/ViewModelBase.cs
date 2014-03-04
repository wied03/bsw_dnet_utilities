using Microsoft.Practices.Prism.ViewModel;

namespace Bsw.Wpf.Utilities.ViewModels
{
    // code credited to http://www.codeproject.com/Articles/97564/Attributes-based-Validation-in-a-WPF-MVVM-Applicat

    /// <summary>
    ///     Base class for all ViewModel classes in the application. Provides support for
    ///     property changes notification.
    /// </summary>
    public abstract class ViewModelBase : NotificationObject
    {
    }
}