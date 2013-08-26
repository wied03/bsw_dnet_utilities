// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

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