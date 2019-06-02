using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class UShortTweaker : Tweaker<ushort>
    {
		public UShortTweaker(ushort from, ushort to, Action<ushort> action) : base(from, to, action) { }
        public override ushort Evaluate(float value, Formula formula) => formula.Calculate(From, To, value);
    }
}