#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MsBw.MsBwUtility.Net.Socket;
using NUnit.Framework;

#endregion

namespace MsBwUtilityTest.Net
{
    [TestFixture]
    public class VariableDelimiterTcpClientTest : BaseTest
    {
        private const int DUMMY_LOCAL_PORT = 5000;
        private TcpClient _tcpClient;
        private List<string> _responsesReceived;
        private VariableDelimiterTcpClient _client;
        private CancellationTokenSource _cts;
        private TcpClient _dummyClient;
        private NetworkStream _dummyStream;
        private StreamWriter _dummyWriter;
        private StreamReader _dummyReader;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _tcpClient = new TcpClient();
            _responsesReceived = new List<string>();
            _client = null;
            _dummyClient = null;
            _dummyStream = null;
            _dummyWriter = null;
            _dummyReader = null;
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public override void TearDown()
        {
            if (_client != null)
            {
                _client.Close();
            }
            base.TearDown();
        }

        private void ConnectTcpClient()
        {
            _tcpClient.Connect(hostname: "localhost",
                               port: DUMMY_LOCAL_PORT);
        }

        protected void StartWritableSocket()
        {
            const int portWeCanWriteStuffTo = DUMMY_LOCAL_PORT + 1;
            StartSocat("TCP4-LISTEN:{0} TCP4-LISTEN:{1}",
                       portWeCanWriteStuffTo,
                       DUMMY_LOCAL_PORT);
            _dummyClient = new TcpClient("localhost",
                                         portWeCanWriteStuffTo);
            _dummyStream = _dummyClient.GetStream();
            _dummyWriter = new StreamWriter(_dummyStream) {AutoFlush = true};
            _dummyReader = new StreamReader(_dummyStream);
        }

        private static Process StartSocat(string argsFormat,
                                          params object[] args)
        {
            var arguments = string.Format(argsFormat,
                                          args);
            var path = Path.GetFullPath("../../Utility/socat-1.7.2.1/socat.exe");
            var procStart = new ProcessStartInfo
                {
                    FileName = path,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = arguments
                };
            var process = Process.Start(procStart);
            return process;
        }

        private void ClientResponseReceived(string response)
        {
            _responsesReceived.Add(response);
        }

        private void SetupClient()
        {
            _client.ResponseReceived += ClientResponseReceived;
            Task.Factory.StartNew(() => _client.ResponseLoop(_cts.Token));
        }

        [Test]
        public void Log_scrub_works_properly()
        {
            // arrange
            StartWritableSocket();
            ConnectTcpClient();
            _client = new VariableDelimiterTcpClient(client: _tcpClient,
                                                     defaultTerminator: "THE_END",
                                                     haltResponseWaitOnKeyword: "BIG_ERROR",
                                                     scrubThisFromLogs: "NSA");
            SetupClient();

            // act
            _client.Send(new DummyRequest {Flat = "Login with NSA password"});

            // assert
            var text = _dummyReader.ReadLine();
            text
                .Should()
                .Be("Login with NSA password");

            TargetForTesting
                .LogMessages
                .ShouldBeEquivalentTo(new[]
                    {
                        "Sending Login with *** password",
                        "Beginning response wait",
                        "Calling stream read"
                    });
        }
    }
}