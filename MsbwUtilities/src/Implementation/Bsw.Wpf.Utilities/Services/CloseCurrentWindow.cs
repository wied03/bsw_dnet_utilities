// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Bsw.Wpf.Utilities.Services
{
    public class CloseCurrentWindow : ICloseCurrentWindow
    {
        readonly IFetchCurrentWindow _currentWindowFetcher;

        public CloseCurrentWindow(IFetchCurrentWindow currentWindowFetcher)
        {
            _currentWindowFetcher = currentWindowFetcher;
        }

        public void Close()
        {
            _currentWindowFetcher.CurrentWindow.Close();
        }
    }
}