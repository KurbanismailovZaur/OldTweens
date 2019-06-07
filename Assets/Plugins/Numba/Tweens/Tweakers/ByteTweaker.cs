using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ByteTweaker : Tweaker<byte>
    {
        public ByteTweaker(byte from, byte to, Action<byte> action) : base(from, to, action) { }

        public override byte Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? Formula.Linear.Calculate(From, To, value);
    }
}