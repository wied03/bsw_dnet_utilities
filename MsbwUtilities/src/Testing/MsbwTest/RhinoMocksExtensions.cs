// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Castle.Core.Interceptor;
using MsBw.MsBwUtility;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace MsbwTest
{
    public static class RhinoMocksExtensions
    {
        public static void SetupPropertyBehaviorOnAll(this object mock)
        {
            SetupPropertyBehaviorOnAllExcept(mock);
        }

        public static void SetupPropertyBehaviorOnAllExcept<TTypeUnderTest>(this TTypeUnderTest mock,
                                                                            params
                                                                                Expression<Func<TTypeUnderTest, object>>
                                                                                [] lambdas) where TTypeUnderTest : class
        {
            var excludeProps = lambdas.Select(l => l.ToPropertyInfo());
            var mockedInterface = mock.MockedInterface();
            var props = mockedInterface.GetProperties()
                .Except(excludeProps)
                ;
            props.Where(p => p.CanRead && p.CanWrite).ToList().ForEach(p => mock.Stub(o => p.GetValue(o))
                                                                                .PropertyBehavior());
        }

        public static Type MockedInterface(this object mock)
        {
            return mock.GetType()
                       .GetInterfaces()
                       .First(it => it != typeof (IMockedObject) &&
                                    it != typeof (ISerializable) &&
                                    it != typeof (IProxyTargetAccessor));
        }
    }
}