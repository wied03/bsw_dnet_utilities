// Copyright 2014 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using MsBw.MsBwUtility.Enum;
using TechTalk.SpecFlow;

namespace Bsw.Utilities.Windows.SystemTest.Transformations
{
    public enum ThatIs
    {
        [StringValue("in any direction")] InAnyDirection,
        [StringValue("to the right of")] ToRightOf,
        [StringValue("underneath")] Underneath,
        [StringValue("above")] Above,
        [StringValue("to the left of")] ToLeftOf
    }

    [Binding]
    public class Directions
    {
        // make it easier for SpecFlow to match this
        [StepArgumentTransformation("(in any direction)")]
        [StepArgumentTransformation("(to the right of)")]
        [StepArgumentTransformation("(underneath)")]
        [StepArgumentTransformation("(above)")]
        [StepArgumentTransformation("(to the left of)")]
        public ThatIs GetAnyDir(string descrip)
        {
            return descrip.EnumValue<ThatIs>();
        }
    }
}