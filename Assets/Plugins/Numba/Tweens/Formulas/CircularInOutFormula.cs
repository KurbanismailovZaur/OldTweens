using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class CircularInOutFormula : Formula
    {
        public override float RemapFormula(float value)
        {
            if (value < 0.5f)
                return 0.5f * (1f - Mathf.Sqrt(1f - 4f * (value * value)));
            else
                return 0.5f * (Mathf.Sqrt(-((2f * value) - 3f) * ((2f * value) - 1f)) + 1f);
        }
    }
}