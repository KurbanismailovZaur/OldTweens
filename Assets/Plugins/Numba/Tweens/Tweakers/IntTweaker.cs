using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class IntTweaker : Tweak<int>
    {
        public override int Evaluate(int from, int to, float value, Formula formula) => formula?.Calculate(from, to, value) ?? Formula.Linear.Calculate(from, to, value);
    }
}