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