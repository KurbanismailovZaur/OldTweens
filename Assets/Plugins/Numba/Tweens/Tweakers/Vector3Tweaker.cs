using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Vector3Tweaker : Tweak<Vector3>
    {
        public override Vector3 Evaluate(Vector3 from, Vector3 to, float value, Formula formula) => formula?.Calculate(from, to, value) ?? Formula.Linear.Calculate(from, to, value);
    }
}