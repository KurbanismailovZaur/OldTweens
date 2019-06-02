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
        public CharTweaker(char from, char to, Action<char> action) : base(from, to, action) { }

        public override char Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}