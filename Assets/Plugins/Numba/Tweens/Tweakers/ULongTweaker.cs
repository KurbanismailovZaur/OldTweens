using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class ULongTweaker : Tweaker<ulong>
    {
        public ULongTweaker(ulong from, ulong to, Action<ulong> action) : base(from, to, action) { }

        public override ulong Evaluate(float value, Formula formula) => formula?.Calculate(From, To, value) ?? throw new ArgumentNullException("Formula can't be a null");
    }
}