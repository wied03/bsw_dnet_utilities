// <copyright file="WindowClosingBehavior.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Bsw.Coworking.Agent.Config.Utilities.Behaviors
{
    public class WindowClosingBehavior
    {
        public static ICommand GetClosed(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ClosedProperty);
        }

        public static void SetClosed(DependencyObject obj,
                                     ICommand value)
        {
            obj.SetValue(ClosedProperty,
                         value);
        }

        public static readonly DependencyProperty ClosedProperty = DependencyProperty.RegisterAttached(
                                                                                                       "Closed",
                                                                                                       typeof (ICommand),
                                                                                                       typeof (
                                                                                                           WindowClosingBehavior
                                                                                                           ),
                                                                                                       new UIPropertyMetadata
                                                                                                           (ClosedChanged));

        private static void ClosedChanged(DependencyObject target,
                                          DependencyPropertyChangedEventArgs e)
        {
            var window = target as Window;

            if (window == null) return;
            if (e.NewValue != null)
            {
                window.Closed += WindowClosed;
            }
            else
            {
                window.Closed -= WindowClosed;
            }
        }

        public static ICommand GetClosing(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ClosingProperty);
        }

        public static void SetClosing(DependencyObject obj,
                                      ICommand value)
        {
            obj.SetValue(ClosingProperty,
                         value);
        }

        public static readonly DependencyProperty ClosingProperty = DependencyProperty.RegisterAttached(
                                                                                                        "Closing",
                                                                                                        typeof (ICommand
                                                                                                            ),
                                                                                                        typeof (
                                                                                                            WindowClosingBehavior
                                                                                                            ),
                                                                                                        new UIPropertyMetadata
                                                                                                            (ClosingChanged));

        private static void ClosingChanged(DependencyObject target,
                                           DependencyPropertyChangedEventArgs e)
        {
            var window = target as Window;

            if (window == null) return;
            if (e.NewValue != null)
            {
                window.Closing += WindowClosing;
            }
            else
            {
                window.Closing -= WindowClosing;
            }
        }

        public static ICommand GetCancelClosing(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(CancelClosingProperty);
        }

        public static void SetCancelClosing(DependencyObject obj,
                                            ICommand value)
        {
            obj.SetValue(CancelClosingProperty,
                         value);
        }

        public static readonly DependencyProperty CancelClosingProperty = DependencyProperty.RegisterAttached(
                                                                                                              "CancelClosing",
                                                                                                              typeof (
                                                                                                                  ICommand
                                                                                                                  ),
                                                                                                              typeof (
                                                                                                                  WindowClosingBehavior
                                                                                                                  ));

        private static void WindowClosed(object sender,
                                         EventArgs e)
        {
            var closed = GetClosed(sender as Window);
            if (closed != null)
            {
                closed.Execute(null);
            }
        }

        private static void WindowClosing(object sender,
                                          CancelEventArgs e)
        {
            var closing = GetClosing(sender as Window);
            if (closing == null) return;
            if (closing.CanExecute(null))
            {
                closing.Execute(null);
            }
            else
            {
                var cancelClosing = GetCancelClosing(sender as Window);
                if (cancelClosing != null)
                {
                    cancelClosing.Execute(null);
                }

                e.Cancel = true;
            }
        }
    }
}