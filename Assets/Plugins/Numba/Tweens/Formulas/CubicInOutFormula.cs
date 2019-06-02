using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class CubicInOutFormula : Formula
    {
        public override float RemapFormula(float value)
        {
            if (value < 0.5f) 
                return 4f * value * value * value;
            else
            {
                var f = ((2f * value) - 2f);
                return 0.5f * f * f * f + 1f;
            }
        }
    }
}