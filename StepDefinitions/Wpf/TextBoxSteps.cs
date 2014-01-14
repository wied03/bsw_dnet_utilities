using System;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class TextBoxSteps : WpfBaseSteps
    {
        [Then(@"that textbox has contents '(.*)'")]
        public void ThenThatTextboxHasContents(string contents)
        {
            var box = Context.TextBox;
            box.Should()
               .NotBeNull();
            box.Text
               .Should()
               .Be(contents);
        }

        [When(@"I enter '(.*)' into that textbox")]
        public void WhenIEnterIntoThatTextbox(string contents)
        {
            var box = Context.TextBox;
            box.Should()
               .NotBeNull();
            box.Text = contents;
        }
    }
}