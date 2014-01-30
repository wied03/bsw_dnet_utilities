// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
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
        public virtual TimeSpan? NumberOfRetrySeconds { get; set; }
        public virtual Application App { get; set; }
        public virtual Window Window { get; set; }
        public virtual Button Button { get; set; }
        public virtual WPFComboBox ComboBox { get; set; }
        public virtual ListView Grid { get; set; }
        public virtual TextBox TextBox { get; set; }
        public virtual CheckBox CheckBox { get; set; }
    }
}