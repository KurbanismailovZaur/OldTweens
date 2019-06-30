using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class QuaternionTweaker : Tweaker<Quaternion>
    {
        public QuaternionTweaker(Func<Quaternion> from, Func<Quaternion> to, Action<Quaternion> action) : base(from, to, action) { }

        public override Quaternion Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}