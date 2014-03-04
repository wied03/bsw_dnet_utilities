// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.Transformations
{
    [Binding]
    public class JsonTransformation
    {
        [StepArgumentTransformation(@"(\{.*\})")]
        public IDictionary<string, object> FromJsonToDynamicObject(string json)
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
        }
    }
}