using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class SByteTweaker : Tweaker<sbyte>
    {
		public SByteTweaker(sbyte from, sbyte to, Action<sbyte> action) : base(from, to, action) { }
        public override sbyte Evaluate(float value, Formula formula) => formula.Calculate(From, To, value);
    }
}