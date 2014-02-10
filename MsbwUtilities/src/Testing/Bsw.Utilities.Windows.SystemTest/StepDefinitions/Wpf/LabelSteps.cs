// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class LabelSteps : WpfBaseSteps
    {
        public static string GetThenThereIsALabelThatSays(string labelText)
        {
            return string.Format(FORMAT_THERE_IS_A_LABEL_THAT_SAYS,
                                 labelText);
        }

        const string FORMAT_THERE_IS_A_LABEL_THAT_SAYS = @"there is a label that says '{0}'";

        [Then(@"there is a label that says '(.*)'")]
        public void ThenThereIsALabelThatSays(string labelText)
        {
            var label = RetryLocate(() => Context.Window.Get<Label>(searchCriteria: SearchCriteria.ByText(labelText)));
            label
                .Should()
                .NotBeNull();
        }
    }
}