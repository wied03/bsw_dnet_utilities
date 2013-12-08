// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;

namespace Bsw.Utilities.Windows.SystemTest
{
    public class GeneralScenarioContext
    {
        public virtual ServiceController ServiceController { get; set; }
    }
}