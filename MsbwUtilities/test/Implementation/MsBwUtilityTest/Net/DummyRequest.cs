// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using MsBw.MsBwUtility.Net.Socket;

#endregion

namespace MsBwUtilityTest.Net
{
    public class DummyRequest : IAmARequest
    {
        public string Flat { get; set; }
    }
}