// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Castle.Core.Interceptor;
using MsBw.MsBwUtility;
using MsBw.MsBwUtility.Tasks;
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

        public static IMethodOptions<Task> StubAsyncVoid<T>(this T obj,
                                                            Function<T, Task> action) where T : class
        {
            var methodOptions = obj.Stub(action);
            methodOptions.Return(true.ToTaskResult());
            return methodOptions;
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
            return methodOptions.Return(executor());
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
            return methodOptions.Do(executor);
        }

        public static IMethodOptions<Task<TReturnType>> DoAsync<TReturnType>(
            this IMethodOptions<Task<TReturnType>> methodOptions,
            Func<TReturnType> action,
            TimeSpan? delayBeforeYourAction = null)
        {
            Func<Task<TReturnType>> executor = async () =>
                                                     {
                                                         if (delayBeforeYourAction != null)
                                                         {
                                                             await Task.Delay(delayBeforeYourAction.Value);
                                                         }
                                                         return action();
                                                     };
            return methodOptions.Do(executor);
        }

        public static IMethodOptions<Task<TReturnType>> DoAsync<TParam1, TReturnType>(
            this IMethodOptions<Task<TReturnType>> methodOptions,
            Func<TParam1, TReturnType> action,
            TimeSpan? delayBeforeYourAction = null)
        {
            Func<TParam1, Task<TReturnType>> executor = async p1 =>
                                                              {
                                                                  if (delayBeforeYourAction != null)
                                                                  {
                                                                      await Task.Delay(delayBeforeYourAction.Value);
                                                                  }
                                                                  return action(p1);
                                                              };
            return methodOptions.Do(executor);
        }

        public static IMethodOptions<Task<TReturnType>> DoAsync<TParam1, TParam2, TReturnType>(
            this IMethodOptions<Task<TReturnType>> methodOptions,
            Func<TParam1, TParam2, TReturnType> action,
            TimeSpan? delayBeforeYourAction = null)
        {
            Func<TParam1, TParam2, Task<TReturnType>> executor = async (p1,
                                                                        p2) =>
                                                                       {
                                                                           if (delayBeforeYourAction != null)
                                                                           {
                                                                               await
                                                                                   Task.Delay(
                                                                                              delayBeforeYourAction
                                                                                                  .Value);
                                                                           }
                                                                           return action(p1,
                                                                                         p2);
                                                                       };
            return methodOptions.Do(executor);
        }

        public static IMethodOptions<Task<TReturnType>> DoAsync<TParam1, TParam2, TParam3, TReturnType>(
            this IMethodOptions<Task<TReturnType>> methodOptions,
            Func<TParam1, TParam2, TParam3, TReturnType> action,
            TimeSpan? delayBeforeYourAction = null)
        {
            throw new NotImplementedException();
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
            return methodOptions.Do(executor);
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
            return methodOptions.Do(executor);
        }

        public static IMethodOptions<Task> DoAsyncVoid<T1, T2, T3>(this IMethodOptions<Task> methodOptions,
                                                                   Action<T1, T2, T3> action,
                                                                   TimeSpan? delayBeforeYourAction = null)
        {
            throw new NotImplementedException();
        }
    }
}