using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class QuarticOutFormula : Formula
    {
        public override float Remap(float value)
        {
            var f = (value - 1f);
            return f * f * f * (1f - value) + 1f;
        }
    }
}