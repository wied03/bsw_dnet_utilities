using System;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace Bsw.Coworking.Agent.Config.Sys.Test.StepDefinitions.Wpf
{
    [Binding]
    public class ButtonSteps : WpfBaseSteps
    {
        private Window Window
        {
            get { return Context.Window; }
        }

        [When(@"I click the '(.*)' button")]
        public void WhenIClickTheButton(string p0)
        {
            var button = Window.Get<Button>(SearchCriteria.ByText(p0));
            button.Click();
        }

        [When(@"I click the '(.*)' button and wait for the window to load")]
        public void WhenIClickTheButtonAndWaitForTheWindowToLoad(string p0)
        {
            var button = Window.Get<Button>(SearchCriteria.ByText(p0));
            button.Click();
            When(ProgressBarSteps.ActionWhenWaitForProgressBarToFinish());
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
            GetButtonAndStoreInContext(buttonText,
                                       labelNearest);
        }

        private Button GetButtonAndStoreInContext(string buttonText,
                                                  string labelNearest)
        {
            var button = LocateClosestElementOfType<Button>(labelNearest,
                                                                        buttonText);
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
            var button = GetButtonAndStoreInContext(buttonText,
                                                    labelNearest);
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