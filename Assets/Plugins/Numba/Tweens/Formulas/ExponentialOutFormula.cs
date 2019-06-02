using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class ExponentialOutFormula : Formula
    {
        public override float RemapFormula(float value) => (value == 1f) ? value : 1f - Mathf.Pow(2f, -10f * value);
    }
}