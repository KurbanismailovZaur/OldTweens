using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class BoolTweaker : Tweaker<bool>
    {
		public BoolTweaker(bool from, bool to, Action<bool> action) : base(from, to, action) { }
        public override bool Evaluate(float value, Formula formula) => formula.Calculate(From, To, value);
    }
}