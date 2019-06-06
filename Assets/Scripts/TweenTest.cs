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
	public abstract class MyClass{
		protected internal float a;
	}

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
            _tween = new Tween(new FloatTweaker(0f, 2f, (x) => _cube1.SetPositionX(x)), 4f, Formula.ExpoOut, 1);
			
        }

        private void Update()
        {
			_tween.SetTime1(_time, false);
        }
    }
}