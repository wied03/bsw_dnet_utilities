using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.StepDefinitions.Util
{
    [Binding]
    public class FileSteps
    {
        [Then(@"the directory '(.*)' has '(.*)' in it")]
        public void ThenTheDirectoryHasInIt(string directoryName,
                                            string fileName)
        {
            Directory.Exists(directoryName)
                     .Should()
                     .BeTrue("Expected {0} directory to exist but it didn't",
                             directoryName);
            var relativePath = Path.Combine(directoryName,
                                            fileName);
            var fullPath = Path.GetFullPath(relativePath);
            var filesInDir = Directory.GetFileSystemEntries(directoryName);
            var fileList = filesInDir.Any()
                               ? filesInDir
                                     .Aggregate((f1,
                                                 f2) => f1 + ", " + f2)
                               : "(no files directory)";
            File.Exists(fullPath)
                .Should()
                .BeTrue("Expected {0} file to exist but it didn't, files that are in there are [{1}]",
                        fullPath,
                        fileList);
        }
    }
}