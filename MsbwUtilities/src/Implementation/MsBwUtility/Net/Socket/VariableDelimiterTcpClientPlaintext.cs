﻿#region

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;

#endregion

namespace MsBw.MsBwUtility.Net.Socket
{
    public class VariableDelimiterTcpClientPlaintext : VariableDelimiterTcpClientBase
    {
        private readonly StreamWriter _writer;

        public VariableDelimiterTcpClientPlaintext(TcpClient client,
                                                   string defaultTerminator,
                                                   string haltResponseWaitOnKeyword) : base(client,
                                                                                            defaultTerminator,
                                                                                            haltResponseWaitOnKeyword)
        {
            _writer = new StreamWriter(Stream) { AutoFlush = true };
        }

        protected override void DoSend(string message)
        {
            _writer.WriteLine(message);
        }
    }
}