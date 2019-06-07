using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class IntTweaker : Tweaker<int>
    {
        public IntTweaker(int from, int to, Action<int> action) : base(from, to, action) { }

        public override int Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? Formula.Linear.Calculate(From, To, value);
    }
}