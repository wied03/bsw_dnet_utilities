﻿#region

using System;
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
        private const string TERMINATOR = "THE_END";
        private const string ERROR = "BIG_ERROR";
        private const string SCRUB = "NSA";
        private TcpClient _tcpClient;
        private VariableDelimiterTcpClient _client;
        private CancellationTokenSource _cts;
        private TcpClient _dummyClient;
        private NetworkStream _dummyStream;
        private StreamWriter _dummyWriter;
        private StreamReader _dummyReader;
        private TaskCompletionSource<string> _taskCompleted;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _tcpClient = new TcpClient();
            _client = null;
            _dummyClient = null;
            _dummyStream = null;
            _dummyWriter = null;
            _dummyReader = null;
            _cts = new CancellationTokenSource();
            _taskCompleted = null;
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

        #endregion

        #region Utility Methods

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
            _taskCompleted.SetResult(response);
        }

        private void SetupResponseWait()
        {
            _taskCompleted = new TaskCompletionSource<string>();
        }

        private void SetupClient()
        {
            _client.ResponseReceived += ClientResponseReceived;
            Task.Factory.StartNew(() => _client.ResponseLoop(_cts.Token));
        }


        private void SetupVariableDelimiterClient()
        {
            _client = new VariableDelimiterTcpClient(client: _tcpClient,
                                                     defaultTerminator: TERMINATOR,
                                                     haltResponseWaitOnKeyword: ERROR,
                                                     scrubThisFromLogs: SCRUB);
        }

        #endregion

        #region Tests

        [Test]
        public void Log_scrub_works_properly_on_send()
        {
            // arrange
            StartWritableSocket();
            ConnectTcpClient();
            SetupVariableDelimiterClient();
            SetupClient();
            const string textToSend = "Login with " + SCRUB + " password";

            // act
            
            _client.Send(new DummyRequest {Flat = textToSend});

            // assert
            // should still get non-scrubbed request on other side
            var text = _dummyReader.ReadLine();
            text
                .Should()
                .Be(textToSend);

            TargetForTesting
                .LogMessages
                .Should()
                .NotContain(logMsg => logMsg.Contains(SCRUB));
        }

        [Test]
        public async Task Log_scrub_works_properly_on_receive()
        {
            // arrange
            StartWritableSocket();
            ConnectTcpClient();
            SetupVariableDelimiterClient();
            SetupClient();
            const string textToReceive = "Login with " + SCRUB + " password " + TERMINATOR;

            // act
            SetupResponseWait();
            _dummyWriter.Write(textToReceive);

            // assert
            var response = await _taskCompleted.Task;
            // should still get the correct text
            response
                .Should()
                .Be(textToReceive);

            TargetForTesting
                .LogMessages
                .Should()
                .NotContain(logMsg => logMsg.Contains(SCRUB));
        }

        #endregion
    }
}