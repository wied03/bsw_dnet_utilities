using System;
using System.Collections.Generic;
using System.Linq;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TestStack.White.UIItems.ListBoxItems;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class ComboBoxSteps : DefaultWpfSteps
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
            var comboBox = Context.ComboBox;
            var itemsText = Context
                .ComboBox
                .Items
                .Select(i => i.Text).ToList();
            if (!itemsText.Contains(itemText))
            {
                var validItems = itemsText.Aggregate((i1,
                                                      i2) => i1 + ", " + i2);
                Assert.Fail(
                            "The item you tried to select in the dropdown ({0}) was not found in the list of dropdown options [{1}]",
                            itemText,
                            validItems);
            }
            comboBox.Select(itemText);
        }

        [Then(@"that combobox is set to '(.*)'")]
        public void ThenThatComboboxIsSetTo(string itemText)
        {
            Context.ComboBox.SelectedItemText
                   .Should()
                   .Be(itemText);
        }

        [Then(@"there is a combobox (.*) label '(.*)'")]
        public void ThenThereIsAComboboxToTheRightOfLabel(ThatIs direction,
                                                          string labelText)
        {
            FindComboBox(direction,
                         labelText);
            Context.ComboBox
                   .Should()
                   .NotBeNull();
        }

        [When(@"I use the combobox (.*) label '(.*)'")]
        public void WhenIUseTheComboboxNearLabel(ThatIs direction,
                                                 string labelText)
        {
            FindComboBox(direction,
                         labelText);
            Context.ComboBox
                   .Should()
                   .NotBeNull();
        }

        void FindComboBox(ThatIs direction,
                          string labelText)
        {
            Context.ComboBox = RetryLocate(() => LocateClosestElementOfType<WPFComboBox>(labelText: labelText,
                                                                                         direction: direction));
        }
    }
}