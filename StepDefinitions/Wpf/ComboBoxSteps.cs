using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Coworking.Agent.Config.Sys.Test.StepDefinitions.Wpf
{
    [Binding]
    public class ComboBoxSteps : WpfBaseSteps
    {
        [Then(@"that combobox has options (.*)")]
        public void ThenThatComboboxHasOptions(IEnumerable<string> groups)
        {
            var itemsText = Context
                .ComboBox
                .Items
                .Select(i => i.Text);
            itemsText
                .Should()
                .BeEquivalentTo(groups);
        }

        [When(@"I select '(.*)'")]
        public void WhenISelect(string itemText)
        {
            Context.ComboBox.Select(itemText);
        }
    }
}