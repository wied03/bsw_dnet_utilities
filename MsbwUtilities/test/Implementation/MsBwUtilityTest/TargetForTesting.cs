#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NLog.Targets;

#endregion

namespace MsBwUtilityTest
{
    [Target("TargetForTesting")]
    public class TargetForTesting : TargetWithLayout
    {
        static TargetForTesting()
        {
            LogMessages = new List<string>();
        }

        public static List<string> LogMessages { get; private set; }

        protected override void Write(NLog.LogEventInfo logEvent)
        {
            LogMessages.Add(logEvent.FormattedMessage);
        }
    }
}