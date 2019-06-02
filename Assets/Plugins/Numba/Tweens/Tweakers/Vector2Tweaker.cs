using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Vector2Tweaker : Tweaker<Vector2>
    {
        public Vector2Tweaker(Vector2 from, Vector2 to, Action<Vector2> action) : base(from, to, action) { }

        public override Vector2 Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}