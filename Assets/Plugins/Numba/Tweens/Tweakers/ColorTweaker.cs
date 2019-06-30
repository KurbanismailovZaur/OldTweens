using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ColorTweaker : Tweaker<Color>
    {
        public ColorTweaker(Func<Color> from, Func<Color> to, Action<Color> action) : base(from, to, action) { }

        public override Color Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}