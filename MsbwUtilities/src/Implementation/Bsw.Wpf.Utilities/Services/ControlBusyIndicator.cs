// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;

namespace Bsw.Wpf.Utilities.Services
{
    public class ControlBusyIndicator : IControlBusyIndicator
    {
        readonly Lazy<BusyIndicator> _indicator;

        BusyIndicator Indicator
        {
            get { return _indicator.Value; }
        }

        public ControlBusyIndicator(IFetchCurrentWindow currentWindowFetcher)
        {
            _indicator = new Lazy<BusyIndicator>(() =>
                                                 {
                                                     var activeWindow = currentWindowFetcher.CurrentWindow;
                                                     return FindBusyIndicator(activeWindow);
                                                 });
        }

        static BusyIndicator FindBusyIndicator(DependencyObject view)
        {
            var indicator = view.FindVisualChildren<BusyIndicator>()
                                .FirstOrDefault();

            if (indicator != null)
            {
                return indicator;
            }

            throw new Exception("Unable to find a BusyIndicator in your XAML window");
        }

        public IControlBusyIndicator Show(string text = "Loading...")
        {
            Indicator.BusyContent = text;
            Indicator.IsBusy = true;
            return this;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        // provides a convenient way to scope the busy indicator
        void Dispose(bool managedAlso)
        {
            if (!managedAlso) return;
            Indicator.IsBusy = false;
        }
    }
}