using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Numba.Tweens;
using Numba.Tweens.Tweakers;
using Numba.Extensions;

namespace Namespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private Tweaker _tweaker;

		[SerializeField]
		[Range(0f, 1f)]
		private float _value;

        private void Start() => _tweaker = new FloatTweaker(1, 3, (i) => _target.SetPositionX(i));

        private void Update() => _tweaker.Apply(_value);
    }
}