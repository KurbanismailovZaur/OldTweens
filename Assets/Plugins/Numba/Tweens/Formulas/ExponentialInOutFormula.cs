using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class ExponentialInOutFormula : Formula
    {
        public override float Remap(float value)
        {
            if (value == 0f || value == 1f) return value;

            return value < 0.5f ? 0.5f * Mathf.Pow(2f, (20f * value) - 10f) : -0.5f * Mathf.Pow(2f, (-20f * value) + 10f) + 1f;
        }
    }
}