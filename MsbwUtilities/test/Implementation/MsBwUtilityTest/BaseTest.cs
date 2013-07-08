#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NLog.Config;
using NUnit.Framework;

#endregion

namespace MsBwUtilityTest
{
    public class BaseTest
    {
        [SetUp]
        public virtual void SetUp()
        {
            ConfigurationItemFactory.Default.Targets.RegisterDefinition("TargetForTesting",typeof(TargetForTesting));
        }

        [TearDown]
        public virtual void TearDown()
        {
        }
    }
}