using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class TextBoxSteps : WpfBaseSteps
    {
        [Then(@"there is a textbox (.*) label '(.*)'")]
        public void ThenThereIsATextboxToTheRightOf(ThatIs direction,
                                                    string labelText)
        {
            FindTextBox(direction,
                        labelText);
            Context.TextBox
                   .Should()
                   .NotBeNull();
        }

        private void FindTextBox(ThatIs direction,
                                 string labelText)
        {
            Context.TextBox = RetryLocate(() => LocateClosestElementOfType<TextBox>(labelText: labelText,
                                                                                    direction: direction));
        }

        [When(@"I use the textbox (.*) label '(.*)'")]
        public void WhenIUseTheTextboxNearLabel(ThatIs direction,
                                                string labelText)
        {
            FindTextBox(direction,
                        labelText);
            Context.TextBox
                   .Should()
                   .NotBeNull();
        }

        [Then(@"that textbox is empty")]
        public void ThenThatTextboxIsEmpty()
        {
            Context.TextBox
                   .Text
                   .Should()
                   .BeNullOrEmpty();
        }

        [Then(@"that textbox is a password box")]
        public void ThenThatTextboxIsAPasswordBox()
        {
            var text = Context.TextBox;
            // ReSharper disable once UnusedVariable
            // need to access the property to trigger the exception
            text.Invoking(t => { var foo = t.Text; })
                .ShouldThrow<WhiteException>("Password boxes throw WhiteExceptions when text access is attempted")
                .WithMessage("Text cannot be retrieved from textbox which has secret text (e.g. password) stored in it",
                             "Password boxes accessed with White framework throw an exception with this message when text access is attempted");
        }

        [Then(@"that textbox only allows hex characters")]
        public void ThenThatTextboxOnlyAllowsHexCharacters()
        {
            var text = Context.TextBox;
            Action<string> check = s =>
            {
                text.Text = s;
                text.Text
                    .Should()
                    .BeEmpty("{0} is not a hex character",
                             s);
            };
            for (var c = 'g'; c < 'z'; c++)
            {
                var str = c.ToString(CultureInfo.InvariantCulture);
                check(str);
                check(str.ToUpper());
            }

            char[] specialCharSpotCheck = { ';', '!', '.', '"' };
            foreach (var c in specialCharSpotCheck)
            {
                var str = c.ToString(CultureInfo.InvariantCulture);
                check(str);
            }
        }

        [Then(@"that textbox only allows integer characters")]
        public void ThenThatTextboxOnlyAllowsIntegerCharacters()
        {
            var text = Context.TextBox;
            Action<string> check = s =>
            {
                text.Text = s;
                text.Text
                    .Trim()
                    .Should()
                    .BeEmpty("{0} is not an integer",
                             s);
            };
            for (var c = 'a'; c < 'z'; c++)
            {
                var str = c.ToString(CultureInfo.InvariantCulture);
                check(str);
                check(str.ToUpper());
            }

            char[] specialCharSpotCheck = { ';', '!', '.', '"' };
            foreach (var c in specialCharSpotCheck)
            {
                var str = c.ToString(CultureInfo.InvariantCulture);
                check(str);
            }
        }

        [Then(@"that textbox is not empty")]
        public void ThenThatTextboxIsNotEmpty()
        {
            Context.TextBox
                   .Text
                   .Should()
                   .NotBeNullOrEmpty();
        }

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