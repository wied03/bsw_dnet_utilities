// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Automation;
using FluentAssertions;
using TestStack.White.UIItems.WindowItems;

namespace Bsw.Utilities.Windows.SystemTest.WhiteClassExtensions
{
    public static class WindowExtensionMethods
    {
        public static void MoveWindowToUpperLeftToEnsureFarRightElementsWork(this Window window,
                                                                             int x = 10,
                                                                             int y = 10)
        {
            var autoElement = window.AutomationElement;
            var transformPattern = (TransformPattern) autoElement.GetCurrentPattern(TransformPattern.Pattern);
            transformPattern.Current.CanMove
                            .Should()
                            .BeTrue();
            transformPattern.Move(x,
                                  y);
        }
    }
}