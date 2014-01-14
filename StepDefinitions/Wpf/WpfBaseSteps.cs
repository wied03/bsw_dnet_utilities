// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Utility;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    public abstract class WpfBaseSteps : BaseSteps<WpfScenarioContext>
    {
        private Window Window
        {
            get { return Context.Window; }
        }

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
                             Context.NumberOfRetrySeconds);
        }

        protected TUiType LocateClosestElementOfType<TUiType>(string labelText) where TUiType : UIItem
        {
            return RetryLocate(() =>
                               {
                                   var window = Window;
                                   var label = window.Get<Label>(SearchCriteria.ByText(labelText));
                                   var theType = typeof (TUiType);
                                   var possibleItems =
                                       window.GetMultiple(SearchCriteria.ByControlType(testControlType: theType,
                                                                                       framework: WindowsFramework.Wpf));
                                   return FindClosest<TUiType>(label,
                                                               possibleItems);
                               });
        }

        private static TUiType FindClosest<TUiType>(IUIItem label,
                                                    IEnumerable<IUIItem> possibleItems) where TUiType : UIItem
        {
            var labelPosition = label.Location;
            var pair = (from item in possibleItems
                        where item.Location.Y > label.Location.Y
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
                                                              string widgetText) where TUiType : UIItem
        {
            return RetryLocate(() =>
                               {
                                   var window = Window;
                                   var label = window.Get<Label>(SearchCriteria.ByText(labelText));
                                   var possibleItems = window.GetMultiple(SearchCriteria.ByText(widgetText));
                                   return FindClosest<TUiType>(label,
                                                               possibleItems);
                               });
        }
    }
}