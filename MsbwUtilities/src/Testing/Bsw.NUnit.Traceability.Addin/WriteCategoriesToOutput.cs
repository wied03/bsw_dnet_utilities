// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Bsw.NUnit.Traceability.Addin
{
    public class WriteCategoriesToOutput : IWriteCategoriesToOutput
    {
        public void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
}