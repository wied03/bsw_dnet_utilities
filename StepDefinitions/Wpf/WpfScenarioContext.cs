// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MsBw.MsBwUtility.JetBrains.Annotations;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.WindowItems;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [UsedImplicitly]
    public class WpfScenarioContext : GeneralScenarioContext
    {
        public TimeSpan NumberOfRetrySeconds
        {
            get
            {
                if (!Context.ContainsKey("numberofretryseconds"))
                {
                    // 10 second default
                    NumberOfRetrySeconds = 10.Seconds();
                }
                return Context.Get<TimeSpan>("numberofretryseconds");
            }
            set { Context["numberofretryseconds"] = value; }
        }

        public Application App
        {
            get { return Context.Get<Application>("app"); }
            set { Context["app"] = value; }
        }

        public Window Window
        {
            get { return Context.Get<Window>("window"); }
            set { Context["window"] = value; }
        }

        public Button Button
        {
            get { return Context.Get<Button>("button"); }
            set { Context["button"] = value; }
        }

        public WPFComboBox ComboBox
        {
            get { return Context.Get<WPFComboBox>("combobox"); }
            set { Context["combobox"] = value; }
        }

        public ListView Grid
        {
            get { return Context.Get<ListView>("grid"); }
            set { Context["grid"] = value; }
        }

        public TextBox TextBox
        {
            get { return Context.Get<TextBox>("textbox"); }
            set { Context["textbox"] = value; }
        }
    }
}