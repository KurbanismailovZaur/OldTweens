using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class Color32Tweaker : Tweaker<Color32>
    {
        public Color32Tweaker(Color32 from, Color32 to, Action<Color32> action) : base(from, to, action) { }

        public override Color32 Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? Formula.Linear.Calculate(From, To, value);
    }
}