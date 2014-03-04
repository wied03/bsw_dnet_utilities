// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Bsw.Wpf.Utilities.Services
{
    public class FetchCurrentWindow : IFetchCurrentWindow
    {
        public Window CurrentWindow
        {
            get
            {
                var active = GetActiveWindow();
                return Application.Current.Windows.OfType<Window>()
                                  .SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
    }
}