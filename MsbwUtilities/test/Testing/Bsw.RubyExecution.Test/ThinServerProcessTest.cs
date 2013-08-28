// // Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using FluentAssertions;

#endregion

namespace Bsw.RubyExecution.Test
{
    [TestFixture]
    public class ThinServerProcessTest : BaseTest
    {
        private TcpListener _tcpListener;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            // avoid any real dependencies on environments
            Environment.SetEnvironmentVariable("PATH",".");
            FullPathToCommandExecuted = Path.GetFullPath(Path.Combine(".",
                                                                       COMMAND_EXECUTED_TXT));
        }

        [TearDown]
        public override void TearDown()
        {
            _tcpListener.Stop();
            base.TearDown();
        }

        #endregion

        #region Utility Methods

        #endregion

        #region Tests

        [Test]
        public void No_ssl_start()
        {
            // arrange
            var thin = new ThinServerProcess(thinPort:1224,
                                             workingDirectory:".");
            _tcpListener = new TcpListener(IPAddress.Any,
                                           1224);
            _tcpListener.Start();
            
            // act
            thin.StartThinServer();
            thin.RubyProc.WaitForExit();

            // assert
            CommandExecuted
                .Should()
                .Be(@".\thin  start -R config.ru -p 1224 -V");
        }

        [Test]
        public void Ssl_start()
        {
            // arrange
            var thin = new ThinServerProcess(thinPort: 1224,
                                             workingDirectory: ".")
                       {
                           ThinSslCertFile = "certfile",
                           ThinSslKeyFile = "keyfile"
                       };
            _tcpListener = new TcpListener(IPAddress.Any,
                                           1224);
            _tcpListener.Start();

            // act
            thin.StartThinServer();
            thin.RubyProc.WaitForExit();

            // assert
            CommandExecuted
                .Should()
                .Be(@".\thin  start -R config.ru -p 1224 -V --ssl --ssl-key-file keyfile --ssl-cert-file certfile");
        }

        [Test]
        public void Shutdown()
        {
            // arrange
            var thin = new ThinServerProcess(thinPort: 1224,
                                             workingDirectory: ".");
            _tcpListener = new TcpListener(IPAddress.Any,
                                           1224);
            _tcpListener.Start();

            // act
            thin.StartThinServer();
            thin.GracefulShutdown();

            // assert
            // because of the quick exit, our shutdown file should already be gone
            var exists = File.Exists(thin.ShutdownTriggerPath);
            exists
                .Should()
                .BeFalse();
        }

        #endregion
    }
}