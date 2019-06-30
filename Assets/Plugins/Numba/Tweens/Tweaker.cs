﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public abstract class Tweaker
    {
        public abstract void Apply(float value, Formula formula = null);
    }

    public abstract class Tweaker<T> : Tweaker where T : struct
    {
        public Func<T> From { get; set; }

        public Func<T> To { get; set; }

        public Action<T> Action { get; set; }

        public Tweaker(Func<T> from, Func<T> to, Action<T> action)
        {
            From = from;
            To = to;
            Action = action;
        }

        public abstract T Evaluate(float value, Formula formula = null);

        public override void Apply(float value, Formula formula = null)
        {
            try
            {
                Action?.Invoke(Evaluate(value, formula));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}