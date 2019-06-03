using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class BackOutFormula : Formula
    {
        public override float Remap(float value)
        {
            var f = (1f - value);
            return 1f - (f * f * f - f * Mathf.Sin(f * Mathf.PI));
        }
    }
}