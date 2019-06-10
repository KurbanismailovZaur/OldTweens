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

        private Tween _tween;

        [SerializeField]
        [Range(1, 4)]
        private int _count = 1;

        [SerializeField]
        private LoopType _loopType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _time;

        private IEnumerator Start()
        {
            _tween = new Tween(new FloatTweaker(0f, 1f, x => _cube1.SetPositionX(x)), 1f, Formula.ExpoInOut, _count, _loopType);
            _tween.PlayRepeated();

            // yield return new WaitForSeconds(3f);

            // _tween.Pause();

            // _tween.Play();

            yield return null;
        }

        private void Update()
        {
            // _tween.SetTimeYEAAAHH(_time);
        }
    }
}