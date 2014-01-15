using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

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

        private List<TextBox> GetTextboxes()
        {
            var type = typeof (TextBox);
            var criteria = SearchCriteria.ByControlType(testControlType: type,
                                                        framework: WindowsFramework.Wpf);
            var textboxes = Context.Window.GetMultiple(criteria)
                                   .Cast<TextBox>()
                                   .ToList()
                ;
            return textboxes;
        }

        [Then(@"there are (.*) textboxes visible")]
        public void ThenThereAreTextboxesVisible(int totalTextBoxes)
        {
            var textboxes = GetTextboxes();
            textboxes
                .Should()
                .HaveCount(totalTextBoxes);
        }

        [Then(@"I select textbox (.*)")]
        public void ThenISelectTextbox(int textBoxIndexFromZero)
        {
            GetTextbox(textBoxIndexFromZero);
        }

        private void GetTextbox(int textBoxIndexFromZero)
        {
            var textboxes = GetTextboxes();
            textBoxIndexFromZero
                .Should()
                .BeLessThan(textboxes.Count,
                            "We can't access a textbox at an index >= the length of the textbox");
            Context.TextBox = textboxes[textBoxIndexFromZero];
        }

        [When(@"use textbox (.*)")]
        public void WhenUseTextbox(int textBoxIndexFromZero)
        {
            GetTextbox(textBoxIndexFromZero);
        }
    }
}