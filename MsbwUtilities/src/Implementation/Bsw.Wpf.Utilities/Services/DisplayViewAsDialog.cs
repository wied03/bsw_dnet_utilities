// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace Bsw.Coworking.Agent.Config.Utilities.Services
{
    public class DisplayViewAsDialog : IDisplayViewAsDialog
    {
        public bool? Display<TViewType, TViewModelType>(TViewModelType viewModel) where TViewType : Window, new()
        {
            var view = new TViewType
                       {
                           DataContext = viewModel
                       };
            return view.ShowDialog();
        }
    }
}