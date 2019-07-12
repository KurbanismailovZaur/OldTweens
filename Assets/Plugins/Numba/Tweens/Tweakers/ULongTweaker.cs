using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ULongTweaker : Tweak<ulong>
    {
        public override ulong Evaluate(ulong from, ulong to, float value, Formula formula) => formula?.Calculate(from, to, value) ?? Formula.Linear.Calculate(from, to, value);
    }
}