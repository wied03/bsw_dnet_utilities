﻿// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Bsw.Wpf.Utilities.ViewModels;

namespace Bsw.Wpf.Utilities.Controls
{
    // can't be abstract or WPF designer complains
    public class WindowWithAsyncInit : Window
    {
        protected WindowWithAsyncInit()
        {
            Loaded += WindowWithAsyncInitLoaded;
        }

        async void WindowWithAsyncInitLoaded(object sender,
                                             RoutedEventArgs e)
        {
            var viewModel = DataContext as IHaveAsyncLoadActions;
            // if i don't check this for null, this can cause problems in the designer
            if (viewModel != null)
            {
                await viewModel.Init();
            }
        }
    }
}