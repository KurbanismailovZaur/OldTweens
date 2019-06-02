using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class RectTweaker : Tweaker<Rect>
    {
        public RectTweaker(Rect from, Rect to, Action<Rect> action) : base(from, to, action) { }

        public override Rect Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}