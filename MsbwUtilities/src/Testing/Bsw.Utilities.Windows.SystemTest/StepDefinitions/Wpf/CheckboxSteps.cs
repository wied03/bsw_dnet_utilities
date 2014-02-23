using System;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class CheckboxSteps : DefaultWpfSteps
    {
        [When(@"I tick the checkbox next to '(.*)'")]
        public void WhenITickTheCheckboxNextTo(string labelText)
        {
            var checkbox = FindCheckbox(labelText);
            checkbox.Checked = true;
        }

        [When(@"I untick the checkbox next to '(.*)'")]
        public void WhenIUntickTheCheckboxNextTo(string labelText)
        {
            var checkbox = FindCheckbox(labelText);
            checkbox.Checked = false;
        }

        CheckBox FindCheckbox(string labelText)
        {
            var checkbox = Context.Window.Get<CheckBox>(SearchCriteria.ByText(labelText));
            checkbox.Should()
                    .NotBeNull();
            return checkbox;
        }

        [Then(@"there is a checkbox next to '(.*)'")]
        public void ThenThereIsACheckboxNextTo(string labelText)
        {
            Context.CheckBox = FindCheckbox(labelText);
        }

        [Then(@"that checkbox is ticked")]
        public void ThenThatCheckboxIsTicked()
        {
            var checkbox = Context.CheckBox;
            checkbox
                .Checked
                .Should()
                .BeTrue();
        }

        [Then(@"that checkbox is not ticked")]
        public void ThenThatCheckboxIsNotTicked()
        {
            var checkbox = Context.CheckBox;
            checkbox
                .Checked
                .Should()
                .BeFalse();
        }
    }
}