using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class LongTweaker : Tweaker<long>
    {
        public LongTweaker(long from, long to, Action<long> action) : base(from, to, action) { }

        public override long Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? Formula.Linear.Calculate(From, To, value);
    }
}