using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public abstract class Tweak<T> where T : struct
    {
        public abstract T Evaluate(T from, T to, float value, Formula formula = null);

        public void Apply(T from, T to, float interpolation, Action<T> setter, Formula formula = null)
        {
            try
            {
                setter?.Invoke(Evaluate(from, to, interpolation, formula));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}