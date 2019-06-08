using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens;
using Numba.Tweens.Tweakers;
using Numba.Extensions;

namespace Namespace
{
    public class TweenTest : MonoBehaviour
    {
        [SerializeField]
        private Transform _cube1;

        [SerializeField]
        [Range(-2f, 7f)]
        private float _time;

        private Tween _tween;

        private void Start()
        {
            new Tween(new FloatTweaker(0f, 1f, x => _cube1.SetPositionX(x)), 1f, Formula.ExpoInOut).PlayIncrementalRepeated();
            new Tween(new FloatTweaker(1f, 2f, s => _cube1.localScale = new Vector3(s, s, s)), 1f, Formula.BackInOut, 2).PlayIncrementalRepeated(4);
        }
    }
}