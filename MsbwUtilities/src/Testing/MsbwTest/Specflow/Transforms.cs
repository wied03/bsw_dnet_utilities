#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MsbwTest.JetBrains.Annotations;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

#endregion

namespace MsbwTest.Specflow
{
    [Binding]
    public class Transforms
    {
        private static readonly Regex QuoteRegex = new Regex(@"'(.*)'");

        [StepArgumentTransformation(@"\[(.*)\]")]
        [UsedImplicitly]
        public IEnumerable<String> StringTransform(string csvItems)
        {
            return csvItems.Split(',')
                           .Select(str => QuoteRegex.Match(str).Groups[1].Value);
        }
    }
}