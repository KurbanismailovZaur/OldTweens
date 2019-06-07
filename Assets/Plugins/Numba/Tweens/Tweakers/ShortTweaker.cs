using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ShortTweaker : Tweaker<short>
    {
        public ShortTweaker(short from, short to, Action<short> action) : base(from, to, action) { }

        public override short Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? Formula.Linear.Calculate(From, To, value);
    }
}