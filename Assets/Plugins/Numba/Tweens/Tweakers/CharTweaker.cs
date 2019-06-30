using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class CharTweaker : Tweaker<char>
    {
        public CharTweaker(Func<char> from, Func<char> to, Action<char> action) : base(from, to, action) { }

        public override char Evaluate(float value, Formula formula) => formula?.Calculate(From(), To(), value) ?? Formula.Linear.Calculate(From(), To(), value);
    }
}