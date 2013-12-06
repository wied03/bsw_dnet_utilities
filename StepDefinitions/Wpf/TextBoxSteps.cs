using System;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Coworking.Agent.Config.Sys.Test.StepDefinitions.Wpf
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
    }
}