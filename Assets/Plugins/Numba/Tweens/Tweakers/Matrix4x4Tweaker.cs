using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Matrix4x4Tweaker : Tweak<Matrix4x4>
    {
        public override Matrix4x4 Evaluate(Matrix4x4 from, Matrix4x4 to, float value, Formula formula) => formula?.Calculate(from, to, value) ?? Formula.Linear.Calculate(from, to, value);
    }
}