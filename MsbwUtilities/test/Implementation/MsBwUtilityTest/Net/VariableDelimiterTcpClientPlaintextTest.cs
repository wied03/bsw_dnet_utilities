// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

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
    public class VariableDelimiterTcpClientPlaintextTest : BaseTest
    {
        #region Fields for test objects

        private const int DUMMY_LOCAL_PORT = 5000;
        private TcpClient _tcpClient;
        private VariableDelimiterTcpClientBase _client;
        private CancellationTokenSource _cts;
        private TcpClient _dummyClient;
        private NetworkStream _dummyStream;
        private StreamWriter _dummyWriter;
        private StreamReader _dummyReader;
        private TaskCompletionSource<string> _taskCompleted;
        private Process _process;

        #endregion

        #region Test Data

        private const string TERMINATOR = "THE_END";
        private const string ERROR = "BIG_ERROR";
        private const string SCRUB = "NSA";

        #endregion

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TargetForTesting.ClearLogMessages();
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
            _process.Kill();
            base.TearDown();
        }

        #endregion

        #region Utility Methods

        private void ConnectTcpClient()
        {
            _tcpClient.Connect(hostname: "localhost",
                               port: DUMMY_LOCAL_PORT);
        }

        private void StartWritableSocket()
        {
            const int portWeCanWriteStuffTo = DUMMY_LOCAL_PORT + 1;
            _process = StartSocat("TCP4-LISTEN:{0} TCP4-LISTEN:{1}",
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

        private void SetupResponseLoop()
        {
            _client.ResponseReceived += ClientResponseReceived;
            Task.Factory.StartNew(() => _client.ResponseLoop(_cts.Token));
        }


        private void SetupVariableDelimiterClient(string scrubFromLogs = SCRUB)
        {
            _client = new VariableDelimiterTcpClientPlaintext(client: _tcpClient,
                                                              defaultTerminator: TERMINATOR,
                                                              haltResponseWaitOnKeyword: ERROR)
                {
                    ScrubThisFromLogs = scrubFromLogs
                };
        }

        private static IEnumerable<string> LogMessages
        {
            get
            {
                return TargetForTesting
                    .LogMessageSnapshot;
            }
        }

        private void Disconnect()
        {
            _cts.Cancel();
            _client.Close();
        }

        #endregion

        #region Tests

        [Test]
        public void No_scrub_configured()
        {
            // arrange
            StartWritableSocket();
            ConnectTcpClient();
            SetupVariableDelimiterClient(scrubFromLogs: null);
            SetupResponseLoop();
            const string textToSend = "Hello";

            // act
            _client.Send(new DummyRequest {Flat = textToSend});

            // assert
            var text = _dummyReader.ReadLine();
            text
                .Should()
                .Be(textToSend);

            Disconnect();
            LogMessages
                .Should()
                .Contain(logMsg => logMsg.Contains("Hello"));
        }


        [Test]
        public void Log_scrub_works_properly_on_send()
        {
            // arrange
            StartWritableSocket();
            ConnectTcpClient();
            SetupVariableDelimiterClient();
            SetupResponseLoop();
            const string textToSend = "Login with " + SCRUB + " password";

            // act
            _client.Send(new DummyRequest {Flat = textToSend});

            // assert
            // should still get non-scrubbed request on other side
            var text = _dummyReader.ReadLine();
            text
                .Should()
                .Be(textToSend);

            Disconnect();
            LogMessages
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
            SetupResponseLoop();
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
            Disconnect();
            LogMessages
                .Should()
                .NotContain(logMsg => logMsg.Contains(SCRUB));
        }

        #endregion
    }
}