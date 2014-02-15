#region

using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

// // Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#endregion

namespace Bsw.RubyExecution
{
    public class ThinServerProcess : RubyProcess
    {
        public int ThinPort { get; private set; }
        public string ThinSslKeyFile { get; set; }
        public string ThinSslCertFile { get; set; }
        public string ShutdownTriggerPath { get; private set; }
        public bool Started { get; private set; }

        public ThinServerProcess(int thinPort,
                                 string workingDirectory) : base(workingDirectory)
        {
            ThinPort = thinPort;
            Started = false;
            ShutdownTriggerPath = Path.Combine(WorkingDirectory,
                                               "shutdown.txt");
        }

        void WaitForServerToStart()
        {
            var up = false;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var tcpClient = new TcpClient();
                    var result = tcpClient.ConnectAsync("localhost",
                                                        ThinPort);
                    var noTimeout = result.Wait(TimeSpan.FromSeconds(3));
                    if (!noTimeout) continue;
                    tcpClient.Close();
                    up = true;
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
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

            var thinArgs = String.Format("{0} {1} start -R config.ru -p {2} -V",
                                         ShutdownTriggerPath,
                                         thinPath,
                                         ThinPort);
            var keyFile = ThinSslKeyFile;
            var args = keyFile != null
                           ? String.Format("{0} --ssl --ssl-key-file {1} --ssl-cert-file {2}",
                                           thinArgs,
                                           keyFile,
                                           ThinSslCertFile)
                           : thinArgs;
            StartRubyProcess(args);
            WaitForServerToStart();
            Started = true;
        }

        static string ThinPath
        {
            get
            {
                var irbPath = RubyIrbPath;
                var irbDirectory = Path.GetDirectoryName(irbPath);
                Debug.Assert(irbDirectory != null,
                             "irbDirectory != null");
                // thin is in the same location as IRB
                var thinPath = Path.Combine(irbDirectory,
                                            "thin");
                return thinPath;
            }
        }

        public void GracefulShutdown()
        {
            var file = File.Create(ShutdownTriggerPath);
            RubyProc.WaitForExit();
            RubyProc.Close();
            file.Close();
            File.Delete(ShutdownTriggerPath);
        }
    }
}