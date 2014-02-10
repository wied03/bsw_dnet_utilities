// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Automation;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Actions;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class MessageBoxSteps : WpfBaseSteps
    {
        const string AUTOMATION_ID_XCEED_MESSAGEBOX_TEXT = "MessageText";
        const string AUTOMATION_ID_XCEED_MESSAGEBOX_TITLE = "TitleText";

        [Then(@"a '(.*)' message box appears that says '(.*)'")]
        public void ThenAMessageBoxAppearsThatSays(string title,
                                                   string expectedMessageBoxText)
        {
            var tuple = GetDialogTitleAndText();
            tuple.Item1
                 .Should()
                 .Be(title);
            tuple.Item2
                 .Should()
                 .Be(expectedMessageBoxText);
        }

        Tuple<string, string> GetDialogTitleAndText()
        {
            var dialogWindow = RetryLocate(() => Context.Window.ModalWindows().FirstOrDefault());
            var dialogElement = dialogWindow.AutomationElement;
            Func<string, string> findTextBox = autoId =>
                                               {
                                                   var treeWalker =
                                                       new TreeWalker(
                                                           new PropertyCondition(
                                                               property: AutomationElement.AutomationIdProperty,
                                                               value: autoId));
                                                   var element = treeWalker.GetFirstChild(dialogElement);
                                                   var textbox = new TextBox(automationElement: element,
                                                                             actionListener: new NullActionListener());
                                                   // this is awkward, but Text throws an exception, probably since these are only visible in raw view
                                                   return textbox.Name;
                                               };
            var titleText = findTextBox(AUTOMATION_ID_XCEED_MESSAGEBOX_TITLE);
            var messageBoxText = findTextBox(AUTOMATION_ID_XCEED_MESSAGEBOX_TEXT);
            var tuple = new Tuple<string, string>(titleText,
                                                  messageBoxText);
            dialogWindow.Close();
            return tuple;
        }

        [Then(@"a '(.*)' message box appears that contains '(.*)'")]
        public void ThenAMessageBoxAppearsThatContains(string title,
                                                       string expectedMessageBoxText)
        {
            var tuple = GetDialogTitleAndText();
            tuple.Item1
                 .Should()
                 .Be(title);
            tuple.Item2
                 .Should()
                 .Contain(expectedMessageBoxText);
        }
    }
}