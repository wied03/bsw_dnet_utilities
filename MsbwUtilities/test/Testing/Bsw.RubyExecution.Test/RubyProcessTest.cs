#region

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

#endregion

namespace Bsw.RubyExecution.Test
{
    [TestFixture]
    public class RubyProcessTest : BaseTest
    {
        // populated in batch file
        private const string COMMAND_EXECUTED_TXT = "command_executed.txt";
        private string _fullPathToCommandExecuted;

        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _fullPathToCommandExecuted = null;
        }

        [TearDown]
        public override void TearDown()
        {
            File.Delete(_fullPathToCommandExecuted);
            base.TearDown();
        }

        #endregion

        #region Utility Methods

        private string CommandExecuted
        {
            get
            {
                return File.ReadAllText(_fullPathToCommandExecuted).Trim();
            }
        }

        #endregion

        #region Tests

        [Test]
        public void Install_bundler_dependencies()
        {
            // arrange
            _fullPathToCommandExecuted = Path.GetFullPath(Path.Combine(".",
                                                                       COMMAND_EXECUTED_TXT));

            // act
            RubyProcess.InstallBundlerDependencies();

            // assert
            CommandExecuted
                .Should()
                .Be("install");
        }

        [Test]
        public void Start_ruby_process()
        {
            // arrange
            const string workingDirectory = @"..\..\workingdir";
            _fullPathToCommandExecuted = Path.GetFullPath(Path.Combine(workingDirectory,
                                                                       COMMAND_EXECUTED_TXT));
            var process = new RubyProcess(workingDirectory: workingDirectory);

            // act
            process.StartRubyProcess("shutdownscript script args");
            process.RubyProc.WaitForExit();

            // assert
            CommandExecuted
                .Should()
                .Be("script  args");
        }

        #endregion
    }
}