using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Vector4Tweaker : Tweaker<Vector4>
    {
        public Vector4Tweaker(Func<Vector4> from, Func<Vector4> to, Action<Vector4> action) : base(from, to, action) { }

        public override Vector4 Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}