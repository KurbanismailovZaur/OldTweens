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
        public Vector3Tweaker(Vector3 from, Vector3 to, Action<Vector3> action) : base(from, to, action) { }

        public override Vector3 Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}