﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class SineOutFormula : Formula
    {
        public override float Remap(float value) => Mathf.Sin(value * (Mathf.PI / 2f));
    }
}