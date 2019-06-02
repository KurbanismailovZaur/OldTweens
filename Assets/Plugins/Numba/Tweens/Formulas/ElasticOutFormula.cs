﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class ElasticOutFormula : Formula
    {
        public override float RemapFormula(float value) => Mathf.Sin(-13f * (Mathf.PI / 2f) * (value + 1f)) * Mathf.Pow(2f, -10f * value) + 1f;
    }
}