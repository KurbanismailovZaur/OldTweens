using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Vector3Tweaker : Tweaker<Vector3>
    {
        public Vector3Tweaker(Func<Vector3> from, Func<Vector3> to, Action<Vector3> action) : base(from, to, action) { }

        public override Vector3 Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}