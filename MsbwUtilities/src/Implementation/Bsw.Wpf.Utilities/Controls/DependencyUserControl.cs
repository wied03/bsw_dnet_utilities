// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;
using StructureMap;

namespace Bsw.Coworking.Agent.Config.Utilities.Controls
{
    public abstract class DependencyUserControl : UserControl
    {
        protected DependencyUserControl()
        {
            ObjectFactory.BuildUp(this);
        }
    }
}