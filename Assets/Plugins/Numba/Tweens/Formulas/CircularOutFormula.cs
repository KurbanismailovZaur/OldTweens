using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class CircularOutFormula : Formula
    {
        public override float Remap(float value) => Mathf.Sqrt((2f - value) * value);
    }
}