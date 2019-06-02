using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Matrix4x4Tweaker : Tweaker<Matrix4x4>
    {
        public Matrix4x4Tweaker(Matrix4x4 from, Matrix4x4 to, Action<Matrix4x4> action) : base(from, to, action) { }

        public override Matrix4x4 Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}