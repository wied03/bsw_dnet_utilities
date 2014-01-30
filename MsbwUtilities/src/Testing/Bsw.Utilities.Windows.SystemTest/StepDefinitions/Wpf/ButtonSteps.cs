using System;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Utility;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class ButtonSteps : WpfBaseSteps
    {
        private Window Window
        {
            get { return Context.Window; }
        }

        [When(@"I click the '(.*)' button")]
        public void WhenIClickTheButton(string buttonText)
        {
            var button = Window.Get<Button>(SearchCriteria.ByText(buttonText));
            Retry.For(() => button.Enabled,
                      NumberOfRetrySeconds);
            button.Click();
        }

        [Then(@"that button is clickable")]
        public void ThenThatButtonIsClickable()
        {
            Context.Button
                   .Enabled
                   .Should()
                   .BeTrue();
        }

        [Then(@"there is a '(.*)' button")]
        public void ThenThereIsAButton(string buttonText)
        {
            Context.Button = Window.Get<Button>(SearchCriteria.ByText(buttonText));
        }

        [Then(@"that button is not clickable")]
        public void ThenTheButtonIsNotClickable()
        {
            Context.Button
                   .Enabled
                   .Should()
                   .BeFalse();
        }

        [Then(@"there is a '(.*)' button under the label '(.*)'")]
        public void ThenThereIsAButtonUnderTheLabel(string buttonText,
                                                    string labelNearest)
        {
            GetButtonUnderLabelAndStoreInContext(buttonText,
                                       labelNearest);
        }

        private Button GetButtonUnderLabelAndStoreInContext(string buttonText,
                                                  string labelNearest)
        {
            var button = LocateClosestElementOfType<Button>(labelText: labelNearest,
                                                            widgetText: buttonText,
                                                            direction: ThatIs.Underneath);
            button
                .Should()
                .NotBeNull();
            Context.Button = button;
            return button;
        }

        [When(@"I click the '(.*)' button under the label '(.*)'")]
        public void WhenIClickTheButtonUnderTheLabel(string buttonText,
                                                     string labelNearest)
        {
            var button = Retry.For(() => GetButtonUnderLabelAndStoreInContext(buttonText,
                                                                    labelNearest),
                                   btn => !btn.Enabled,
                                   NumberOfRetrySeconds);
            button.Enabled
                  .Should()
                  .BeTrue("Can't click a button that's not enabled");
            button.Click();
        }

        [When(@"I click the '(.*)' button on row (\d+)")]
        public void WhenIClickTheButtonOnRow(string buttonText,
                                             int rowIndex)
        {
            var action = GridSteps.ActionThenThereIsAButtonOnRow(buttonText,
                                                                 rowIndex);
            Then(action);
            Context.Button.Click();
        }
    }
}