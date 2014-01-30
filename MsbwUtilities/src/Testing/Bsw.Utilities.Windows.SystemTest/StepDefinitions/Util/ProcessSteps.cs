// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using Bsw.Utilities.Windows.SystemTest.StepDefinitions.Wpf;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestStack.White;
using TestStack.White.Utility;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util
{
    [Binding]
    public class ProcessSteps : WpfBaseSteps
    {
        [Then(@"process '(.*)' runs and window '(.*)' appears")]
        public void ThenProcessRunsAndWindowAppears(string processPath,
                                                    string windowTitle)
        {
            var processIds = Retry.For(() => GetProcessIdsWithFilename(processPath).ToList(),
                                       pn => !pn.Any(),
                                       NumberOfRetrySeconds);
            processIds
                .Should()
                .HaveCount(1,
                           "Expected process {0} to be running, but it's not",
                           processPath);
            var processId = processIds.First();
            Console.WriteLine("Found process ID {0}, will now look for window",
                              processId);
            var process = Process.GetProcessById(processId);
            Context.App = Application.Attach(process);
            Context.Window = Context.App.GetWindow(windowTitle);
        }

        [Then(@"no process '(.*)' runs for at least (.*) seconds")]
        public void ThenNoProcessRunsForAtLeastSeconds(string processPath,
                                                       int seconds)
        {
            var processIds = Retry.For(() => GetProcessIdsWithFilename(processPath).ToList(),
                                       pn => !pn.Any(),
                                       seconds.Seconds());
            processIds.Should()
                      .BeEmpty("Expected no process IDs at all with that path");
        }

        public static IEnumerable<int> GetProcessIdsWithFilename(string filename)
        {
            Console.WriteLine("Searching for processes with filename {0}",
                              filename);
            // if we don't escape the backslashes, the query fails
            var escaped = filename.Replace(@"\",
                                           @"\\");
            var query = string.Format("SELECT ProcessId FROM Win32_Process WHERE ExecutablePath = '{0}'",
                                      escaped);
            return SelectProcessIdQuery(query);
        }

        public static IEnumerable<int> GetProcessIdsWithArguments(string arguments)
        {
            var query = string.Format("SELECT ProcessId FROM Win32_Process WHERE CommandLine LIKE '%{0}%'",
                                      arguments);
            return SelectProcessIdQuery(query);
        }

        private static IEnumerable<int> SelectProcessIdQuery(string query)
        {
            var searcher = new ManagementObjectSearcher(query);
            return searcher.Get()
                           .Cast<ManagementObject>()
                           .Select(mo => mo["ProcessId"])
                           .Cast<UInt32>()
                           .Select(Convert.ToInt32);
        }
    }
}