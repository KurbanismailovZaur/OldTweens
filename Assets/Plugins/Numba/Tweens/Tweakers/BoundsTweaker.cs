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
        public BoundsTweaker(Bounds from, Bounds to, Action<Bounds> action) : base(from, to, action) { }

        public override Bounds Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}