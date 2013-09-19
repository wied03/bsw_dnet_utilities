// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MsbwTest;
using NUnit.Framework;

namespace MsbwTest_Test
{
    [TestFixture]
    public class ExtensionMethodsTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        #endregion

        #region Utility Methods

        private async Task DoStuff()
        {
            throw new Exception("foobar");
        }

        private async Task<int> DoOtherStuff()
        {
            throw new Exception("foobar");
        }

        #endregion

        #region Tests

        [Test]
        public async Task Invoking_async_void()
        {
            // arrange + act + assert
            await this.InvokingAsync(t => t.DoStuff())
                      .ShouldThrow<Exception>();
        }

        [Test]
        public async Task Invoking_async_notvoid_exception()
        {
            // arrange + act + assert
            await this.InvokingAsync(t => t.DoOtherStuff())
                      .ShouldThrow<Exception>();
        }

        #endregion
    }
}