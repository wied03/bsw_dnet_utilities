// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util;
using Bsw.Utilities.Windows.SystemTest.Transformations;
using TechTalk.SpecFlow;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.Utility;
using TestStack.White.WindowsAPI;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    public abstract class WpfBaseSteps : BaseSteps<WpfScenarioContext>
    {
        protected TUiType RetryLocate<TUiType>(Func<TUiType> retryAction,
                                               string itemLookingFor = null) where TUiType : UIItem
        {
            var retryCount = 1;
            return Retry.For(() =>
                             {
                                 var lookingFor = itemLookingFor ?? typeof (TUiType).Name;
                                 Console.WriteLine("Looking for {0} try # {1}",
                                                   lookingFor,
                                                   retryCount++);
                                 return retryAction();
                             },
                             t => t == null,
                             NumberOfRetrySeconds);
        }

        protected TimeSpan NumberOfRetrySeconds
        {
            get
            {
                var timeout = Context.NumberOfRetrySeconds;
                if (!timeout.HasValue)
                {
                    throw new Exception(
                        "You must set Context.NumberOfRetrySeconds in a BeforeStep/BeforeScenario method");
                }
                return timeout.Value;
            }
        }

        protected TUiType LocateClosestElementOfType<TUiType>(string labelText,
                                                              ThatIs direction = ThatIs.InAnyDirection)
            where TUiType : UIItem
        {
            return RetryLocate(() =>
                               {
                                   var window = Context.Window;
                                   var label = window.Get<Label>(SearchCriteria.ByText(labelText));
                                   var theType = typeof (TUiType);
                                   var possibleItems =
                                       window.GetMultiple(SearchCriteria.ByControlType(testControlType: theType,
                                                                                       framework: WindowsFramework.Wpf));
                                   return FindClosest<TUiType>(label,
                                                               possibleItems,
                                                               direction);
                               });
        }

        private static TUiType FindClosest<TUiType>(IUIItem label,
                                                    IEnumerable<IUIItem> possibleItems,
                                                    ThatIs direction) where TUiType : UIItem
        {
            var labelPosition = label.Location;
            Func<IUIItem, IUIItem, bool> predicate;
            switch (direction)
            {
                case ThatIs.Underneath:
                    predicate = (item,
                                 lbl) => item.Location.Y > lbl.Location.Y;
                    break;
                case ThatIs.Above:
                    predicate = (item,
                                 lbl) => item.Location.Y < lbl.Location.Y;
                    break;
                case ThatIs.ToLeftOf:
                    predicate = (item,
                                 lbl) => item.Location.X < lbl.Location.X;
                    break;
                case ThatIs.InAnyDirection:
                    predicate = (item,
                                 lbl) => true; // our orderby below takes care of this
                    break;
                case ThatIs.ToRightOf:
                    predicate = (item,
                                 lbl) => item.Location.X > lbl.Location.X;
                    break;
                default:
                    var error =
                        string.Format(
                                      "Unmapped 'ThatIs' value '{0}'.  Edit WpfBaseSteps to add a case statement for this value!",
                                      direction);
                    throw new Exception(error);
            }
            var pair = (from item in possibleItems
                        where predicate(item,
                                        label)
                        // only below
                        select new {widget = item, distance = (item.Location - labelPosition).Length}
                       ).OrderBy(p => p.distance)
                        .FirstOrDefault();
            if (pair == null)
            {
                return null;
            }

            var widget = pair.widget;
            Console.WriteLine(@"Located widget ID:{0} Name: {1} Location: {2}",
                              widget.Id,
                              widget.Name,
                              widget.Location);
            return (TUiType) widget;
        }

        protected TUiType LocateClosestElementOfType<TUiType>(string labelText,
                                                              string widgetText,
                                                              ThatIs direction) where TUiType : UIItem
        {
            return RetryLocate(() =>
                               {
                                   var window = Context.Window;
                                   var label = window.Get<Label>(SearchCriteria.ByText(labelText));
                                   var possibleItems = window.GetMultiple(SearchCriteria.ByText(widgetText));
                                   return FindClosest<TUiType>(label,
                                                               possibleItems,
                                                               direction);
                               });
        }

        [When(@"I press tab to get the focus to change")]
        public void WhenIPressTabToGetTheFocusToChange()
        {
            Context.Window.KeyIn(KeyboardInput.SpecialKeys.TAB);
        }
    }
}