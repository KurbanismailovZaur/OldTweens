﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class DecimalTweaker : Tweaker<decimal>
    {
        public DecimalTweaker(Func<decimal> from, Func<decimal> to, Action<decimal> action) : base(from, to, action) { }

        public override decimal Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}