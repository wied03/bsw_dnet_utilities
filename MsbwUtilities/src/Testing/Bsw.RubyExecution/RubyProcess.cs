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
        private Process _rubyProcess;

        public int ThinPort { get; private set; }
        public string ThinSslKeyFile { get; set; }
        public string ThinSslCertFile { get; set; }
        public string WorkingDirectory { get; private set; }
        private static readonly string ThisBinPath = Path.GetFullPath(@".");
        public string ShutdownTriggerPath { get; private set; }
        public bool Started { get; private set; }

        public RubyProcess(int thinPort,
                           string workingDirectory)
        {
            ThinPort = thinPort;
            WorkingDirectory = workingDirectory;
            Started = false;
            ShutdownTriggerPath = Path.Combine(WorkingDirectory,
                                               "shutdown.txt");
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

        private void WaitForServerToStart()
        {
            var up = false;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var tcpClient = new TcpClient();
                    var result = tcpClient.ConnectAsync("localhost",
                                                        ThinPort);
                    var noTimeout = result.Wait(3.Seconds());
                    if (!noTimeout) continue;
                    tcpClient.Close();
                    up = true;
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(50.Milliseconds());
                }
            }
            if (!up)
            {
                throw new Exception("Tried 10 times to check server uptime and gave up!");
            }
        }

        public void StartThinServer()
        {
            var thinPath = ThinPath;

            var thinArgs = string.Format("{0} {1} start -R config.ru -p {2} -V",
                                         ShutdownTriggerPath,
                                         thinPath,
                                         ThinPort);
            var keyFile = ThinSslKeyFile;
            var args = keyFile != null
                           ? string.Format("{0} --ssl --ssl-key-file {1} --ssl-cert-file {2}",
                                           thinArgs,
                                           keyFile,
                                           ThinSslCertFile)
                           : thinArgs;
            StartRubyProcess(args);
        }

        private static string ThinPath
        {
            get
            {
                var irbPath = RubyIrbPath;
                var thinPath = Path.Combine(Path.GetDirectoryName(irbPath),
                                            "thin");
                return thinPath;
            }
        }

        public void GracefulShutdown()
        {
            var file = File.Create(ShutdownTriggerPath);
            _rubyProcess.WaitForExit();
            _rubyProcess.Close();
            file.Close();
            File.Delete(ShutdownTriggerPath);
        }

        public void KillRubyProcess()
        {
            _rubyProcess.Kill();
        }

        public void StartRubyProcess(string args)
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
            _rubyProcess = new Process {StartInfo = procStart};
            _rubyProcess.OutputDataReceived += (sender,
                                                eventArgs) => Console.WriteLine(eventArgs.Data);
            _rubyProcess.ErrorDataReceived += (sender,
                                               eventArgs) => Console.WriteLine(eventArgs.Data);
            _rubyProcess.Start();
            _rubyProcess.BeginOutputReadLine();
            _rubyProcess.BeginErrorReadLine();
            WaitForServerToStart();
            Started = true;
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