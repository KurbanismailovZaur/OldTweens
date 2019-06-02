﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class UIntTweaker : Tweaker<uint>
    {
		public UIntTweaker(uint from, uint to, Action<uint> action) : base(from, to, action) { }
        public override uint Evaluate(float value, Formula formula) => formula.Calculate(From, To, value);
    }
}