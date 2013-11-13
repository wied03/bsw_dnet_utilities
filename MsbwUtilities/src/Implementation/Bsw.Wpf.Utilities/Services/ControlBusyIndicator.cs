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
        private readonly Lazy<BusyIndicator> _indicator;

        private BusyIndicator Indicator
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

        private static BusyIndicator FindBusyIndicator(DependencyObject view)
        {
            var indicator = view.FindVisualChildren<BusyIndicator>()
                                .FirstOrDefault();

            if (indicator != null)
            {
                return indicator;
            }

            throw new Exception("Unable to find a BusyIndicator in your XAML window");
        }

        public void Show(string text = "Loading...")
        {
            Indicator.BusyContent = text;
            Indicator.IsBusy = true;
        }

        public void Hide()
        {
            Indicator.IsBusy = false;
        }
    }
}