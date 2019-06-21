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
        internal protected abstract object FromObject { get; set; }

        internal protected abstract object ToObject { get; set; }

        internal protected abstract object EvaluateObject(float value, Formula formula = null);

        public abstract void Apply(float value, Formula formula = null);
    }

    public abstract class Tweaker<T> : Tweaker where T : struct
    {
        internal protected override object FromObject
        {
            get => From;
            set => From = (T)value;
        }

        public T From { get; set; }

        internal protected override object ToObject
        {
            get => To;
            set => To = (T)value;
        }

        public T To { get; set; }

        public Action<T> Action { get; set; }

        public Tweaker(T from, T to, Action<T> action)
        {
            From = from;
            To = to;
            Action = action;
        }

        internal protected override object EvaluateObject(float value, Formula formula = null) => Evaluate(value, formula);

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