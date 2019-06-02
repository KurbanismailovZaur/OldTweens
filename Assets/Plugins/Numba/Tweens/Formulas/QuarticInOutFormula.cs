using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class QuarticInOutFormula : Formula
    {
        public override float RemapFormula(float value)
        {
            if (value < 0.5f) 
                return 8f * value * value * value * value;
            else
            {
                var f = (value - 1f);
                return -8f * f * f * f * f + 1f;
            }
        }
    }
}