﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class QuadraticInOutFormula : Formula
    {
        public override float RemapFormula(float value) => (value < 0.5f ? 2f * value * value : (-2f * value * value + (4f * value) - 1f));
    }
}