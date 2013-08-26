// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading;
using FluentAssertions;

#endregion

namespace Bsw.RubyExecution
{
    public class RubyProcess
    {
        protected Process RubyProc { get; private set; }

        public string WorkingDirectory { get; private set; }
        private static readonly string ThisBinPath = Path.GetFullPath(@".");

        public RubyProcess(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        public static string RubyIrbPath
        {
            get
            {
                var sysPath = Environment.GetEnvironmentVariable("PATH");
                Debug.Assert(sysPath != null,
                             "sysPath != null");
                return sysPath
                    .Split(';')
                    .Select(dir => Path.Combine(dir,
                                                "irb.bat"))
                    .First(File.Exists);
            }
        }

        public void KillRubyProcess()
        {
            RubyProc.Kill();
        }

        public virtual void StartRubyProcess(string args)
        {
            // don't want to run this inside of bin
            var executable = Path.GetFullPath(Path.Combine(ThisBinPath,
                                                           "run_ruby.bat"));
            var procStart = new ProcessStartInfo
                            {
                                FileName = executable,
                                Arguments = args,
                                WorkingDirectory = WorkingDirectory,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardInput = true,
                                RedirectStandardError = true,
                            };
            RubyProc = new Process {StartInfo = procStart};
            RubyProc.OutputDataReceived += (sender,
                                                eventArgs) => Console.WriteLine(eventArgs.Data);
            RubyProc.ErrorDataReceived += (sender,
                                               eventArgs) => Console.WriteLine(eventArgs.Data);
            RubyProc.Start();
            RubyProc.BeginOutputReadLine();
            RubyProc.BeginErrorReadLine();
        }

        public static void InstallBundlerDependencies()
        {
            var executable = Path.GetFullPath(Path.Combine(ThisBinPath,
                                                           "bundle_install.bat"));
            var procStart = new ProcessStartInfo
                            {
                                FileName = executable,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                UseShellExecute = false
                            };
            Process.Start(procStart).WaitForExit();
            var bundleLogFile = Path.GetFullPath(Path.Combine(ThisBinPath,
                                                              "bundle_install.log"));
            Console.WriteLine("Bundle install log:");
            Console.WriteLine(File.ReadAllText(bundleLogFile));
        }
        
    }
}