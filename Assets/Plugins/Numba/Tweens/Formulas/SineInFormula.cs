using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class SineInFormula : Formula
    {
        public override float RemapFormula(float value) => Mathf.Sin((value - 1f) * (Mathf.PI / 2f)) + 1f;
    }
}