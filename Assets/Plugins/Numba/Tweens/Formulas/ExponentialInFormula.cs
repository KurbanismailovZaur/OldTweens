using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class ExponentialInFormula : Formula
    {
        public override float RemapFormula(float value) => (value == 0f) ? value : Mathf.Pow(2f, 10f * (value - 1f));
    }
}