using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens.Tweakers
{
    public class DoubleTweaker : Tweaker<double>
    {
		public DoubleTweaker(double from, double to, Action<double> action) : base(from, to, action) { }
        public override double Evaluate(float value, Formula formula) => formula.Calculate(From, To, value);
    }
}