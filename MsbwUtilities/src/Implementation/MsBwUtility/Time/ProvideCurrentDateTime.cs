// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using MsBw.MsBwUtility.JetBrains.Annotations;

namespace MsBw.MsBwUtility.Time
{
    [UsedImplicitly]
    public class ProvideCurrentDateTime : IProvideCurrentDateTime
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTimeOffset NowOffset
        {
            get { return DateTimeOffset.Now; }
        }
    }
}