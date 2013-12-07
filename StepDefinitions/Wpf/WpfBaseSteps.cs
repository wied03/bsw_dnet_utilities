// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf
{
    public abstract class WpfBaseSteps : BaseSteps
    {
        private readonly WpfScenarioContext _wpfContext;

        protected WpfBaseSteps()
        {
            _wpfContext = new WpfScenarioContext();
        }

        protected WpfScenarioContext Context
        {
            get { return _wpfContext; }
        }

        private Window Window
        {
            get { return Context.Window; }
        }

        protected TUiType LocateClosestElementOfType<TUiType>(string labelText) where TUiType : UIItem
        {
            var window = Window;
            var label = window.Get<Label>(SearchCriteria.ByText(labelText));
            var theType = typeof(TUiType);
            var possibleItems = window.GetMultiple(SearchCriteria.ByControlType(testControlType: theType,
                                                                                framework: WindowsFramework.Wpf));
            var labelPosition = label.Location;
            var widget = (from item in possibleItems
                          where item.Location.Y > label.Location.Y
                          // only below
                          select new { widget = item, distance = (item.Location - labelPosition).Length }
                         ).OrderBy(p => p.distance)
                          .First()
                          .widget;
            Console.WriteLine(@"Located widget ID:{0} Name: {1} Location: {2}",
                              widget.Id,
                              widget.Name,
                              widget.Location);
            return (TUiType)widget;
        }

        protected TUiType LocateClosestElementOfType<TUiType>(string labelText,
                                                                  string widgetText) where TUiType : UIItem
        {
            var window = Window;
            var label = window.Get<Label>(SearchCriteria.ByText(labelText));
            Trace.WriteLine("Got label");
            var possibleItems = window.GetMultiple(SearchCriteria.ByText(widgetText));
            var labelPosition = label.Location;
            var widget = (from item in possibleItems
                          where item.Location.Y > label.Location.Y
                          select new { widget = item, distance = (item.Location - labelPosition).Length }
                         ).OrderBy(p => p.distance)
                          .First()
                          .widget;
            Console.WriteLine(@"Located widget ID:{0} Name: {1} Location: {2}",
                              widget.Id,
                              widget.Name,
                              widget.Location);
            return (TUiType)widget;
        }
    }
}