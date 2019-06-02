using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;

namespace Numba.Tweens.Formulas
{
    public class ElasticInOutFormula : Formula
    {
        public override float RemapFormula(float value)
        {
            if (value < 0.5f)
                return 0.5f * Mathf.Sin(13f * (Mathf.PI / 2f) * (2f * value)) * Mathf.Pow(2f, 10f * ((2f * value) - 1f));
            else
                return 0.5f * (Mathf.Sin(-13f * (Mathf.PI / 2f) * ((2f * value - 1f) + 1f)) * Mathf.Pow(2f, -10f * (2f * value - 1f)) + 2f);
        }
    }
}