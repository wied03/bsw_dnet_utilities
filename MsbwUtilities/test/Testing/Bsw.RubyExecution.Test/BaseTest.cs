using System.IO;
using NUnit.Framework;

#region

using System;
using System.Linq;
using System.Linq.Expressions;

#endregion

// // Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
namespace Bsw.RubyExecution.Test
{
    public class BaseTest
    {
        protected const string COMMAND_EXECUTED_TXT = "command_executed.txt";
        protected string FullPathToCommandExecuted;

        [SetUp]
        public virtual void SetUp()
        {
            FullPathToCommandExecuted = null;
        }

        [TearDown]
        public virtual void TearDown()
        {
            File.Delete(FullPathToCommandExecuted);
        }

        protected string CommandExecuted
        {
            get
            {
                return File.ReadAllText(FullPathToCommandExecuted).Trim();
            }
        }
    }
}