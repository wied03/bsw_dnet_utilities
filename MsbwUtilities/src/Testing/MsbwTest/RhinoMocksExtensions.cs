﻿// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
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

        public static IMethodOptions<Task<T>> ReturnAsync<T>(this IMethodOptions<Task<T>> methodOptions,
                                                             T someValue,
                                                             TimeSpan? delay = null)
        {
            Func<Task<T>> executor = async () =>
                                           {
                                               if (delay != null)
                                               {
                                                   await Task.Delay(delay.Value);
                                               }
                                               return someValue;
                                           };
            methodOptions.Return(executor());
            return methodOptions;
        }

        public static IMethodOptions<Task> DoAsyncVoid(this IMethodOptions<Task> methodOptions,
                                                          Action action,
                                                          TimeSpan? delayBeforeYourAction = null)
        {
            Func<Task> executor = async () =>
            {
                if (delayBeforeYourAction != null)
                {
                    await Task.Delay(delayBeforeYourAction.Value);
                }
                action();
            };
            methodOptions.Do(executor);
            return methodOptions;
        }

        public static IMethodOptions<Task> DoAsyncVoid<T>(this IMethodOptions<Task> methodOptions,
                                                          Action<T> action,
                                                          TimeSpan? delayBeforeYourAction = null)
        {
            Func<T, Task> executor = async o =>
                                           {
                                               if (delayBeforeYourAction != null)
                                               {
                                                   await Task.Delay(delayBeforeYourAction.Value);
                                               }
                                               action(o);
                                           };
            methodOptions.Do(executor);
            return methodOptions;
        }

        public static IMethodOptions<Task> DoAsyncVoid<T1, T2>(this IMethodOptions<Task> methodOptions,
                                                               Action<T1, T2> action,
                                                               TimeSpan? delayBeforeYourAction = null)
        {
            Func<T1, T2, Task> executor = async (o1,
                                                 o2) =>
                                                {
                                                    if (delayBeforeYourAction != null)
                                                    {
                                                        await Task.Delay(delayBeforeYourAction.Value);
                                                    }
                                                    action(o1,
                                                           o2);
                                                };
            methodOptions.Do(executor);
            return methodOptions;
        }
    }
}