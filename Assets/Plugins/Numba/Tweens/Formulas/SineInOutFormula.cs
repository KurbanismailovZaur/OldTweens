using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class SineInOutFormula : Formula
    {
        public override float RemapFormula(float value) => 0.5f * (1f - Mathf.Cos(value * Mathf.PI));
    }
}