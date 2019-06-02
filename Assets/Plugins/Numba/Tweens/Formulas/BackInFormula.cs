using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class BackInFormula : Formula
    {
        public override float RemapFormula(float value) => value * value * value - value * Mathf.Sin(value * Mathf.PI);
    }
}