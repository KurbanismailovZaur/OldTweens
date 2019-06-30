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
        private Transform _cube2;

        [SerializeField]
        private Transform _cube3;

        private Tween _tween1;

        private Tween _tween2;

        private Tween _tween3;

        [Header("Tween 1")]
        [SerializeField]
        [Range(1, 4)]
        private int _tween1Count = 1;

        [SerializeField]
        private LoopType _tween1LoopType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _tween1Duration = 1f;

        [Header("Tween 2")]
        [SerializeField]
        [Range(1, 4)]
        private int _tween2Count = 1;

        [SerializeField]
        private LoopType _tween2LoopType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _tween2Duration = 1f;

        [Header("Tween 3")]
        [SerializeField]
        [Range(1, 4)]
        private int _tween3Count = 1;

        [SerializeField]
        private LoopType _tween3LoopType;

        [SerializeField]
        [Range(0f, 1f)]
        private float _tween3Duration = 1f;

        private Sequence _sequence1;

        [Header("Sequence 1")]
        [SerializeField]
        [Range(1, 4)]
        private int _sequence1Count = 1;

        [SerializeField]
        private LoopType _sequence1LoopType;

        private Sequence _sequence2;

        [Header("Sequence 2")]
        [SerializeField]
        [Range(1, 4)]
        private int _sequence2Count = 1;

        [SerializeField]
        private LoopType _sequence2LoopType;

        private Sequence _sequence3;

        [Header("Sequence 3")]
        [SerializeField]
        [Range(1, 4)]
        private int _sequence3Count = 1;

        [SerializeField]
        private LoopType _sequence3LoopType;

        [Header("Common")]
        [SerializeField]
        [Range(-1f, 16f)]
        private float _time;

        private IEnumerator Start()
        {
            var tween = new Tween("Tween 1", new FloatTweaker(0f, 1f, x => _cube1.SetPositionX(x)), 1f, Formula.BounceIn, 2, LoopType.Backward);

            var sequence1 = new Sequence("Sequence 1");
            sequence1.Append(tween);

            var sequence2 = new Sequence("Sequence 2", 1, LoopType.Backward);
            sequence2.Append(sequence1);

            yield return sequence1.Play();
            sequence2.Play();
            
            yield return null;
        }
    }
}