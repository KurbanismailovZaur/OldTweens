using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class BoundsTweaker : Tweaker<Bounds>
    {
        public BoundsTweaker(Func<Bounds> from, Func<Bounds> to, Action<Bounds> action) : base(from, to, action) { }

        public override Bounds Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}