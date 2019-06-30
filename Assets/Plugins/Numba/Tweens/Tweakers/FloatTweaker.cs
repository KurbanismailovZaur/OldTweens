﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class FloatTweaker : Tweaker<float>
    {
        public FloatTweaker(Func<float> from, Func<float> to, Action<float> action) : base(from, to, action) { }
        
        public override float Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}