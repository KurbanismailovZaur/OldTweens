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
    public class TweakerTest : MonoBehaviour
    {
        [SerializeField]
        private Transform _target1;

        [SerializeField]
        private Transform _target2;

        [SerializeField]
        private Transform _target3;

        [SerializeField]
        private Transform _target4;

        [SerializeField]
        private Transform _target5;

        [SerializeField]
        private Transform _target6;

        [SerializeField]
        private Transform _target7;

        [SerializeField]
        private Transform _target8;

        [SerializeField]
        private Transform _target9;

        [SerializeField]
        private Transform _target10;

        [SerializeField]
        private Transform _target11;

        [SerializeField]
        private Transform _target12;

        [SerializeField]
        private Transform _target13;

        [SerializeField]
        private Transform _target14;

        [SerializeField]
        private Transform _target15;

        [SerializeField]
        private Transform _target16;

        [SerializeField]
        private Transform _target17;

        [SerializeField]
        private Transform _target18;

        [SerializeField]
        private Transform _target19;

        [SerializeField]
        private Transform _target20;

        [SerializeField]
        private Transform _target21;

        [SerializeField]
        private Transform _target22;

        [SerializeField]
        private Transform _target23;

        [SerializeField]
        private Transform _target24;

        [SerializeField]
        private Transform _target25;

        [SerializeField]
        private Transform _target26;

        [SerializeField]
        private Transform _target27;

        [SerializeField]
        private Transform _target28;

        [SerializeField]
        private Transform _target29;

        [SerializeField]
        private Transform _target30;

        [SerializeField]
        private Transform _target31;

        [SerializeField]
        [Range(-1f, 2f)]
        private float _interpolation;

        private FloatTweaker _tweaker1;

        private FloatTweaker _tweaker2;

        private FloatTweaker _tweaker3;

        private FloatTweaker _tweaker4;

        private FloatTweaker _tweaker5;

        private FloatTweaker _tweaker6;

        private FloatTweaker _tweaker7;

        private FloatTweaker _tweaker8;

        private FloatTweaker _tweaker9;

        private FloatTweaker _tweaker10;

        private FloatTweaker _tweaker11;

        private FloatTweaker _tweaker12;

        private FloatTweaker _tweaker13;

        private FloatTweaker _tweaker14;

        private FloatTweaker _tweaker15;

        private FloatTweaker _tweaker16;

        private FloatTweaker _tweaker17;

        private FloatTweaker _tweaker18;

        private FloatTweaker _tweaker19;

        private FloatTweaker _tweaker20;

        private FloatTweaker _tweaker21;

        private FloatTweaker _tweaker22;

        private FloatTweaker _tweaker23;

        private FloatTweaker _tweaker24;

        private FloatTweaker _tweaker25;

        private FloatTweaker _tweaker26;

        private FloatTweaker _tweaker27;

        private FloatTweaker _tweaker28;

        private FloatTweaker _tweaker29;

        private FloatTweaker _tweaker30;

        private FloatTweaker _tweaker31;

        private void Start()
        {
            _tweaker1 = new FloatTweaker();
            _tweaker2 = new FloatTweaker();
            _tweaker3 = new FloatTweaker();
            _tweaker4 = new FloatTweaker();
            _tweaker5 = new FloatTweaker();
            _tweaker6 = new FloatTweaker();
            _tweaker7 = new FloatTweaker();
            _tweaker8 = new FloatTweaker();
            _tweaker9 = new FloatTweaker();
            _tweaker10 = new FloatTweaker();
            _tweaker11 = new FloatTweaker();
            _tweaker12 = new FloatTweaker();
            _tweaker13 = new FloatTweaker();
            _tweaker14 = new FloatTweaker();
            _tweaker15 = new FloatTweaker();
            _tweaker16 = new FloatTweaker();
            _tweaker17 = new FloatTweaker();
            _tweaker18 = new FloatTweaker();
            _tweaker19 = new FloatTweaker();
            _tweaker20 = new FloatTweaker();
            _tweaker21 = new FloatTweaker();
            _tweaker22 = new FloatTweaker();
            _tweaker23 = new FloatTweaker();
            _tweaker24 = new FloatTweaker();
            _tweaker25 = new FloatTweaker();
            _tweaker26 = new FloatTweaker();
            _tweaker27 = new FloatTweaker();
            _tweaker28 = new FloatTweaker();
            _tweaker29 = new FloatTweaker();
            _tweaker30 = new FloatTweaker();
            _tweaker31 = new FloatTweaker();
        }

        private void Update()
        {
            _tweaker1.Apply(0f, 1f, _interpolation, x => _target1.SetPositionX(x), Formula.Linear);
            _tweaker2.Apply(0f, 1f, _interpolation, x => _target2.SetPositionX(x), Formula.QuadIn);
            _tweaker3.Apply(0f, 1f, _interpolation, x => _target3.SetPositionX(x), Formula.QuadOut);
            _tweaker4.Apply(0f, 1f, _interpolation, x => _target4.SetPositionX(x), Formula.QuadInOut);
            _tweaker5.Apply(0f, 1f, _interpolation, x => _target5.SetPositionX(x), Formula.CubicIn);
            _tweaker6.Apply(0f, 1f, _interpolation, x => _target6.SetPositionX(x), Formula.CubicOut);
            _tweaker7.Apply(0f, 1f, _interpolation, x => _target7.SetPositionX(x), Formula.CubicInOut);
            _tweaker8.Apply(0f, 1f, _interpolation, x => _target8.SetPositionX(x), Formula.QuartIn);
            _tweaker9.Apply(0f, 1f, _interpolation, x => _target9.SetPositionX(x), Formula.QuartOut);
            _tweaker10.Apply(0f, 1f, _interpolation, x => _target10.SetPositionX(x), Formula.QuartInOut);
            _tweaker11.Apply(0f, 1f, _interpolation, x => _target11.SetPositionX(x), Formula.QuintIn);
            _tweaker12.Apply(0f, 1f, _interpolation, x => _target12.SetPositionX(x), Formula.QuintOut);
            _tweaker13.Apply(0f, 1f, _interpolation, x => _target13.SetPositionX(x), Formula.QuintInOut);
            _tweaker14.Apply(0f, 1f, _interpolation, x => _target14.SetPositionX(x), Formula.SineIn);
            _tweaker15.Apply(0f, 1f, _interpolation, x => _target15.SetPositionX(x), Formula.SineOut);
            _tweaker16.Apply(0f, 1f, _interpolation, x => _target16.SetPositionX(x), Formula.SineInOut);
            _tweaker17.Apply(0f, 1f, _interpolation, x => _target17.SetPositionX(x), Formula.CircIn);
            _tweaker18.Apply(0f, 1f, _interpolation, x => _target18.SetPositionX(x), Formula.CircOut);
            _tweaker19.Apply(0f, 1f, _interpolation, x => _target19.SetPositionX(x), Formula.CircInOut);
            _tweaker20.Apply(0f, 1f, _interpolation, x => _target20.SetPositionX(x), Formula.ExpoIn);
            _tweaker21.Apply(0f, 1f, _interpolation, x => _target21.SetPositionX(x), Formula.ExpoOut);
            _tweaker22.Apply(0f, 1f, _interpolation, x => _target22.SetPositionX(x), Formula.ExpoInOut);
            _tweaker23.Apply(0f, 1f, _interpolation, x => _target23.SetPositionX(x), Formula.ElasticIn);
            _tweaker24.Apply(0f, 1f, _interpolation, x => _target24.SetPositionX(x), Formula.ElasticOut);
            _tweaker25.Apply(0f, 1f, _interpolation, x => _target25.SetPositionX(x), Formula.ElasticInOut);
            _tweaker26.Apply(0f, 1f, _interpolation, x => _target26.SetPositionX(x), Formula.BackIn);
            _tweaker27.Apply(0f, 1f, _interpolation, x => _target27.SetPositionX(x), Formula.BackOut);
            _tweaker28.Apply(0f, 1f, _interpolation, x => _target28.SetPositionX(x), Formula.BackInOut);
            _tweaker29.Apply(0f, 1f, _interpolation, x => _target29.SetPositionX(x), Formula.BounceIn);
            _tweaker30.Apply(0f, 1f, _interpolation, x => _target30.SetPositionX(x), Formula.BounceOut);
            _tweaker31.Apply(0f, 1f, _interpolation, x => _target31.SetPositionX(x), Formula.BounceInOut);
        }
    }
}