// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;

namespace MsBw.MsBwUtility.Time
{
    public interface IProvideCurrentDateTime
    {
        DateTime Now { get; }
        DateTimeOffset NowOffset { get; }
    }
}