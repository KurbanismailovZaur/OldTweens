using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ColorTweaker : Tweak<Color>
    {
        public override Color Evaluate(Color from, Color to, float value, Formula formula) => formula?.Calculate(from, to, value) ?? Formula.Linear.Calculate(from, to, value);
    }
}