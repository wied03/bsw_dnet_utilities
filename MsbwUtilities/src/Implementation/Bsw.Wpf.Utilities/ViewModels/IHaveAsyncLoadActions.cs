// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bsw.Wpf.Utilities.ViewModels
{
    public interface IHaveAsyncLoadActions
    {
        Task Init();
    }
}