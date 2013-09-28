// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Castle.Core.Interceptor;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace MsbwTest
{
    public static class RhinoMocksExtensions
    {
        public static void SetupPropertyBehaviorOnAll(this object mock)
        {
            var mockedInterface = mock.MockedInterface();
            var props = mockedInterface.GetProperties();
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