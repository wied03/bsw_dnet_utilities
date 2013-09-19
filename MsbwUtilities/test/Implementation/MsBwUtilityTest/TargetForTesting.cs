// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NLog;
using NLog.Targets;

#endregion

namespace MsBwUtilityTest
{
    [Target("TargetForTesting")]
    public class TargetForTesting : TargetWithLayout
    {
        private static readonly List<string> LogMessages;

        static TargetForTesting()
        {
            LogMessages = new List<string>();
        }

        public static void ClearLogMessages()
        {
            LogMessages.Clear();
        }

        public static IEnumerable<string> LogMessageSnapshot
        {
            get { return new List<string>(LogMessages); }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            LogMessages.Add(logEvent.FormattedMessage);
        }
    }
}