// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

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
    public abstract class VariableDelimiterTcpClientBase : IDisposable
    {
        protected const int BUFFER_SIZE = 256;
        internal const string SCRUB_PLACEHOLDER = "***";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly TcpClient _client;
        private string _expectedSuccessfulTerminator;
        private string _expectedErrorTerminator;
        private readonly string _haltResponseWaitOnKeyword;
        private readonly string _defaultTerminator;
        public event ResponseReceivedEvent ResponseReceived;
        private readonly StringBuilder _receivedBuffer;
        private bool _closed;
        protected readonly NetworkStream Stream;

        protected VariableDelimiterTcpClientBase(TcpClient client,
                                                 string defaultTerminator,
                                                 string haltResponseWaitOnKeyword)
        {
            _client = client;
            Stream = _client.GetStream();
            _receivedBuffer = new StringBuilder();
            _defaultTerminator = defaultTerminator;
            _haltResponseWaitOnKeyword = haltResponseWaitOnKeyword;
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

        public string ScrubThisFromLogs { get; set; }

        private string ScrubMessage(string message)
        {
            return string.IsNullOrEmpty(ScrubThisFromLogs)
                       ? message
                       : message.Replace(ScrubThisFromLogs,
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

                DoSend(request.Flat);
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

        protected abstract void DoSend(string message);

        private void ProcessReceivedChars(string received)
        {
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

        protected virtual Tuple<byte[], int> DoReadIntoBuffer()
        {
            try
            {
                var buffer = new byte[BUFFER_SIZE];
                var actualRead = Stream.Read(buffer,
                                             0,
                                             BUFFER_SIZE);
                return new Tuple<byte[], int>(buffer,
                                              actualRead);
            }
            catch (ObjectDisposedException)
            {
                Logger.Debug("Socket object disposed, returning and assuming connection is closed");
                return null;
            }
            catch (IOException e)
            {
                var socketError = e.InnerException as SocketException;
                if (socketError == null) throw;
                if (socketError.SocketErrorCode != SocketError.ConnectionReset &&
                    socketError.SocketErrorCode != SocketError.Interrupted)
                {
                    throw;
                }
                Logger.Debug("Socket reset in stream read, returning and assuming connection is closed");
                return null;
            }
        }

        protected virtual string DoRead()
        {
            var readInfo = DoReadIntoBuffer();
            return readInfo == null
                       ? null
                       : Encoding.Default.GetString(readInfo.Item1,
                                                    0,
                                                    readInfo.Item2);
        }

        public void ResponseLoop(CancellationToken token)
        {
            Logger.Trace("Beginning response wait");

            while (!token.IsCancellationRequested && IsConnected)
            {
                Logger.Trace("Calling stream read");
                try
                {
                    var readStuff = DoRead();
                    Logger.Trace("Stream read complete");
                    ProcessReceivedChars(readStuff);
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
                            case SocketError.ConnectionReset:
                            case SocketError.Shutdown:
                            case SocketError.NotSocket:
                                Logger.Debug("Normal socket error '{0}', halting response loop",
                                             socketErrorCode.ToString());
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
                catch (SocketException se)
                {
                    var socketErrorCode = se.SocketErrorCode;
                    switch (socketErrorCode)
                    {
                        case SocketError.NotSocket:
                            return false;
                        default:
                            throw;
                    }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // in case something inherits us and uses native resources
        ~VariableDelimiterTcpClientBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool freeManagedAlso)
        {
            if (!freeManagedAlso || _closed) return;
            _client.Close();
        }
    }
}