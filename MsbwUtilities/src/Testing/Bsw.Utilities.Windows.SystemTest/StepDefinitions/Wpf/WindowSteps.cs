// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class WindowSteps : WpfBaseSteps
    {
        [Then(@"A window titled '(.*)' appears")]
        public void ThenAWindowTitledAppears(string windowText)
        {
            Context.Window.Title
                   .Should()
                   .Be(windowText);
        }
    }
}