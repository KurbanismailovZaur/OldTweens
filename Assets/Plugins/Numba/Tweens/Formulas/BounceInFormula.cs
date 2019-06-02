﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class BounceInFormula : Formula
    {
        public override float RemapFormula(float value) => 1f - BounceOut.RemapFormula(1f - value);
    }
}