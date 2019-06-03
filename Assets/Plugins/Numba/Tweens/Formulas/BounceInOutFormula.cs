using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class BounceInOutFormula : Formula
    {
        public override float Remap(float value)
        {
            if (value < 0.5f)
                return 0.5f * BounceIn.Remap(value * 2f);
            else
                return 0.5f * BounceOut.Remap(value * 2f - 1f) + 0.5f;
        }
    }
}