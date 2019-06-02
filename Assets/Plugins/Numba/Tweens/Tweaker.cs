using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
	public abstract class Tweaker
	{
		public abstract void Apply(float value, Formula formula);
	}

    public abstract class Tweaker<T> : Tweaker where T : struct
    {
        public T From { get; set; }

        public T To { get; set; }

        public Action<T> Action { get; set; }

        public Tweaker(T from, T to, Action<T> action)
        {
            From = from;
            To = to;
            Action = action;
        }

        public abstract T Evaluate(float value, Formula formula);

        public override void Apply(float value, Formula formula) => Action(Evaluate(value, formula));
    }
}