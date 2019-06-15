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

        private Tween _tween1;

        [Header("Tween")]
        [SerializeField]
        [Range(1, 4)]
        private int _tweenCount = 1;

        [SerializeField]
        private LoopType _tweenLoopType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _tweenDuration = 1f;

        private Sequence _sequence1;

        [Header("Sequence")]
        [SerializeField]
        [Range(1, 4)]
        private int _sequenceCount = 1;

        [SerializeField]
        private LoopType _sequenceLoopType;

        [Header("Common")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _time;

        private IEnumerator Start()
        {
            _tween1 = new Tween(new FloatTweaker(0f, 1f, x => _cube1.SetPositionX(x)), _tweenDuration, Formula.ExpoInOut, _tweenCount, _tweenLoopType);
            _tween1.Play(); 

            // _sequence1 = new Sequence("Sequence1", _sequenceCount, _sequenceLoopType);
            // _sequence1.Append(_tween1);

            // _sequence1.Play();

            yield return null;
        }

        private void Update()
        {
            // _tween1.SetTimeIIIIUUUHH(_time);
            // _sequence1.SetTimeIIIUUUHH(_time);
        }
    }
}