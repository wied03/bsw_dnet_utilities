#region

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

#endregion

namespace MsBw.MsBwUtility.Net.Socket
{
    public class VariableDelimiterTcpClient
    {
        private const int BUFFER_SIZE = 256;
        internal const string SCRUB_PLACEHOLDER = "***";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly TcpClient _client;
        private string _expectedSuccessfulTerminator;
        private string _expectedErrorTerminator;
        private readonly string _haltResponseWaitOnKeyword;
        private readonly string _scrubThisFromLogs;
        private readonly string _defaultTerminator;
        private readonly StreamWriter _writer;
        public event ResponseReceivedEvent ResponseReceived;
        private readonly StringBuilder _receivedBuffer;
        private bool _closed;
        private readonly NetworkStream _stream;

        public VariableDelimiterTcpClient(TcpClient client,
                                          string defaultTerminator,
                                          string haltResponseWaitOnKeyword,
                                          string scrubThisFromLogs)
        {
            _client = client;
            _stream = _client.GetStream();
            _writer = new StreamWriter(_stream) {AutoFlush = true};
            _receivedBuffer = new StringBuilder();
            _defaultTerminator = defaultTerminator;
            _haltResponseWaitOnKeyword = haltResponseWaitOnKeyword;
            _scrubThisFromLogs = scrubThisFromLogs;
            _expectedSuccessfulTerminator = defaultTerminator;
            _expectedErrorTerminator = defaultTerminator;
            _closed = false;
        }

        public void ChangeExpectedSuccessTerminator(string terminator)
        {
            Logger.Trace("Set success terminator on next expected result to: '{0}'",
                         terminator);

            _expectedSuccessfulTerminator = terminator;
        }

        public void ChangeExpectedErrorTerminator(string terminator)
        {
            Logger.Trace("Set error terminator on next expected result to: '{0}'",
                         terminator);

            _expectedErrorTerminator = terminator;
        }

        private string ScrubMessage(string message)
        {
            return message.Replace(_scrubThisFromLogs,
                                   SCRUB_PLACEHOLDER);
        }

        public void Send(IAmARequest request)
        {
            try
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("Sending {0}",
                                 ScrubMessage(request.Flat));
                }

                _writer.WriteLine(request.Flat);
            }
            catch (IOException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException != null)
                {
                    throw socketException;
                }
                throw;
            }
        }

        private void ProcessReceivedChars(byte[] receivedDataBuffer,
                                          int bytesRead)
        {
            var received = Encoding.Default.GetString(receivedDataBuffer,
                                                      0,
                                                      bytesRead);
            _receivedBuffer.Append(received);
            if (Logger.IsTraceEnabled)
            {
                var receivedStr = _receivedBuffer.ToString();
                Logger.Trace("Got characters from socket {0}",
                             ScrubMessage(receivedStr));
            }
            var completeResponse = AnalyzeCharactersForTerminators(_receivedBuffer.ToString());
            if (completeResponse == null) return;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Got complete string '{0}' from other side, invoking ResponseReceived event",
                             ScrubMessage(completeResponse));
            }

            ResponseReceived(completeResponse);
            _receivedBuffer.Clear();
        }

        public void ResponseLoop(CancellationToken token)
        {
            Logger.Trace("Beginning response wait");

            while (!token.IsCancellationRequested && IsConnected)
            {
                Logger.Trace("Calling stream read");
                var buffer = new byte[BUFFER_SIZE];
                try
                {
                    var actualRead = _stream.Read(buffer,
                                                  0,
                                                  BUFFER_SIZE);
                    Logger.Trace("Stream read complete");
                    ProcessReceivedChars(buffer,
                                         actualRead);
                }
                catch (ObjectDisposedException)
                {
                    Logger.Debug("Socket interrupted due to closure of client");
                    return;
                }
                catch (IOException e)
                {
                    var socketException = e.InnerException as SocketException;
                    if (socketException != null)
                    {
                        var socketErrorCode = socketException.SocketErrorCode;
                        switch (socketErrorCode)
                        {
                            case SocketError.Interrupted:
                                // We're being closed, just exit
                                Logger.Debug("Socket interrupted due to closure of client, halting response loop");
                                return;
                            case SocketError.ConnectionReset:
                                Logger.Debug("Other side disconnected us, halting response loop");
                                return;
                            case SocketError.Shutdown:
                                Logger.Debug("Socket is already shutdown, halting response loop");
                                return;
                            default:
                                Logger.Debug("Got an unknown socket error code '{0}', allowing rethrow",
                                             socketErrorCode.ToString());
                                break;
                        }
                    }
                    throw;
                }
            }
            Logger.Debug("Response wait ending due to cancellation/shutdown");
        }

        private string AnalyzeCharactersForTerminators(string alreadyFetchedChars)
        {
            var trimmed = alreadyFetchedChars.Trim();
            if (trimmed.Contains(_haltResponseWaitOnKeyword) && trimmed.EndsWith(_expectedErrorTerminator))
            {
                // we probably have an error
                return trimmed;
            }

            if (!trimmed.EndsWith(_expectedSuccessfulTerminator))
            {
                return null;
            }

            // Always make sure we reset back to the original terminator after any temporary requests to use a different one

            Logger.Trace("Resetting terminator from {0} back to {1}",
                         _expectedSuccessfulTerminator,
                         _defaultTerminator);

            _expectedSuccessfulTerminator = _defaultTerminator;
            return trimmed;
        }

        public bool IsConnected
        {
            get
            {
                if (_closed) return false;
                var socket = _client.Client;
                try
                {
                    return !((socket.Poll(1000,
                                          SelectMode.SelectRead) && socket.Available == 0) ||
                             !socket.Connected);
                }
                    // could be in process of closing while we are checking this
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }
        }


        public void Close()
        {
            if (_closed)
            {
                Logger.Trace("Socket has already been closed, not doing anything");
                // we've already been closed
                return;
            }

            Logger.Trace("Shutting down TCP client");
            _client.Close();
            Logger.Trace("Client shutdown complete");
            _closed = true;
        }
    }
}