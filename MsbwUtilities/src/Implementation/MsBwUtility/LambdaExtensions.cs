// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MsBw.MsBwUtility
{
    public static class LambdaExtensions
    {
        public static PropertyInfo ToPropertyInfo<TTypeUnderTest>(this Expression<Func<TTypeUnderTest, object>> lambda)
        {
            var memberExpression = lambda.Body as MemberExpression;
            if (memberExpression != null) return memberExpression.Member as PropertyInfo;
            var unary = (UnaryExpression) lambda.Body;
            memberExpression = (MemberExpression) unary.Operand;
            return memberExpression.Member as PropertyInfo;
        }
    }
}