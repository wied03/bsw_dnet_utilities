// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using TechTalk.SpecFlow;
using TestStack.White.WindowsAPI;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    [Binding]
    public class KeySteps : DefaultWpfSteps
    {
        [When(@"I press tab to get the focus to change")]
        public void WhenIPressTabToGetTheFocusToChange()
        {
            Context.Window.KeyIn(KeyboardInput.SpecialKeys.TAB);
        }
    }
}