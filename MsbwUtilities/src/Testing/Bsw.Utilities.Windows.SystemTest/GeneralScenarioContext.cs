// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bsw.Utilities.Windows.SystemTest
{
    public class GeneralScenarioContext
    {
        public virtual List<string> ServicesStarted { get; set; }
        public virtual DateTimeOffset? NowSnapshot { get; set; }
    }
}