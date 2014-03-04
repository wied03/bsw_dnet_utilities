// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Windows;

namespace Bsw.Wpf.Utilities.Services
{
    public interface IDisplayViewAsDialog
    {
        bool? Display<TViewType, TViewModelType>(TViewModelType viewModel) where TViewType : Window, new();
    }
}