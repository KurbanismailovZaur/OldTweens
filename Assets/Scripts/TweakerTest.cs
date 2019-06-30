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
        private float _value;

        private Tweaker _tweaker1;

        private Tweaker _tweaker2;

        private Tweaker _tweaker3;

        private Tweaker _tweaker4;

        private Tweaker _tweaker5;

        private Tweaker _tweaker6;

        private Tweaker _tweaker7;

        private Tweaker _tweaker8;

        private Tweaker _tweaker9;

        private Tweaker _tweaker10;

        private Tweaker _tweaker11;

        private Tweaker _tweaker12;

        private Tweaker _tweaker13;

        private Tweaker _tweaker14;

        private Tweaker _tweaker15;

        private Tweaker _tweaker16;

        private Tweaker _tweaker17;

        private Tweaker _tweaker18;

        private Tweaker _tweaker19;

        private Tweaker _tweaker20;

        private Tweaker _tweaker21;

        private Tweaker _tweaker22;

        private Tweaker _tweaker23;

        private Tweaker _tweaker24;

        private Tweaker _tweaker25;

        private Tweaker _tweaker26;

        private Tweaker _tweaker27;

        private Tweaker _tweaker28;

        private Tweaker _tweaker29;

        private Tweaker _tweaker30;

        private Tweaker _tweaker31;

        private void Start()
        {
            _tweaker1 = new FloatTweaker(() => 0f, () => 1f, (f) => _target1.SetPositionX(f));
            _tweaker2 = new FloatTweaker(() => 0f, () => 1f, (f) => _target2.SetPositionX(f));
            _tweaker3 = new FloatTweaker(() => 0f, () => 1f, (f) => _target3.SetPositionX(f));
            _tweaker4 = new FloatTweaker(() => 0f, () => 1f, (f) => _target4.SetPositionX(f));
            _tweaker5 = new FloatTweaker(() => 0f, () => 1f, (f) => _target5.SetPositionX(f));
            _tweaker6 = new FloatTweaker(() => 0f, () => 1f, (f) => _target6.SetPositionX(f));
            _tweaker7 = new FloatTweaker(() => 0f, () => 1f, (f) => _target7.SetPositionX(f));
            _tweaker8 = new FloatTweaker(() => 0f, () => 1f, (f) => _target8.SetPositionX(f));
            _tweaker9 = new FloatTweaker(() => 0f, () => 1f, (f) => _target9.SetPositionX(f));
            _tweaker10 = new FloatTweaker(() => 0f, () => 1f, (f) => _target10.SetPositionX(f));
            _tweaker11 = new FloatTweaker(() => 0f, () => 1f, (f) => _target11.SetPositionX(f));
            _tweaker12 = new FloatTweaker(() => 0f, () => 1f, (f) => _target12.SetPositionX(f));
            _tweaker13 = new FloatTweaker(() => 0f, () => 1f, (f) => _target13.SetPositionX(f));
            _tweaker14 = new FloatTweaker(() => 0f, () => 1f, (f) => _target14.SetPositionX(f));
            _tweaker15 = new FloatTweaker(() => 0f, () => 1f, (f) => _target15.SetPositionX(f));
            _tweaker16 = new FloatTweaker(() => 0f, () => 1f, (f) => _target16.SetPositionX(f));
            _tweaker17 = new FloatTweaker(() => 0f, () => 1f, (f) => _target17.SetPositionX(f));
            _tweaker18 = new FloatTweaker(() => 0f, () => 1f, (f) => _target18.SetPositionX(f));
            _tweaker19 = new FloatTweaker(() => 0f, () => 1f, (f) => _target19.SetPositionX(f));
            _tweaker20 = new FloatTweaker(() => 0f, () => 1f, (f) => _target20.SetPositionX(f));
            _tweaker21 = new FloatTweaker(() => 0f, () => 1f, (f) => _target21.SetPositionX(f));
            _tweaker22 = new FloatTweaker(() => 0f, () => 1f, (f) => _target22.SetPositionX(f));
            _tweaker23 = new FloatTweaker(() => 0f, () => 1f, (f) => _target23.SetPositionX(f));
            _tweaker24 = new FloatTweaker(() => 0f, () => 1f, (f) => _target24.SetPositionX(f));
            _tweaker25 = new FloatTweaker(() => 0f, () => 1f, (f) => _target25.SetPositionX(f));
            _tweaker26 = new FloatTweaker(() => 0f, () => 1f, (f) => _target26.SetPositionX(f));
            _tweaker27 = new FloatTweaker(() => 0f, () => 1f, (f) => _target27.SetPositionX(f));
            _tweaker28 = new FloatTweaker(() => 0f, () => 1f, (f) => _target28.SetPositionX(f));
            _tweaker29 = new FloatTweaker(() => 0f, () => 1f, (f) => _target29.SetPositionX(f));
            _tweaker30 = new FloatTweaker(() => 0f, () => 1f, (f) => _target30.SetPositionX(f));
            _tweaker31 = new FloatTweaker(() => 0f, () => 1f, (f) => _target31.SetPositionX(f));
        }

        private void Update()
        {
            _tweaker1.Apply(_value, Formula.Linear);
            _tweaker2.Apply(_value, Formula.QuadIn);
            _tweaker3.Apply(_value, Formula.QuadOut);
            _tweaker4.Apply(_value, Formula.QuadInOut);
            _tweaker5.Apply(_value, Formula.CubicIn);
            _tweaker6.Apply(_value, Formula.CubicOut);
            _tweaker7.Apply(_value, Formula.CubicInOut);
            _tweaker8.Apply(_value, Formula.QuartIn);
            _tweaker9.Apply(_value, Formula.QuartOut);
            _tweaker10.Apply(_value, Formula.QuadInOut);
            _tweaker11.Apply(_value, Formula.QuintIn);
            _tweaker12.Apply(_value, Formula.QuintOut);
            _tweaker13.Apply(_value, Formula.QuintInOut);
            _tweaker14.Apply(_value, Formula.SineIn);
            _tweaker15.Apply(_value, Formula.SineOut);
            _tweaker16.Apply(_value, Formula.SineInOut);
            _tweaker17.Apply(_value, Formula.CircIn);
            _tweaker18.Apply(_value, Formula.CircOut);
            _tweaker19.Apply(_value, Formula.CircInOut);
            _tweaker20.Apply(_value, Formula.ExpoIn);
            _tweaker21.Apply(_value, Formula.ExpoOut);
            _tweaker22.Apply(_value, Formula.ExpoInOut);
            _tweaker23.Apply(_value, Formula.ElasticIn);
            _tweaker24.Apply(_value, Formula.ElasticOut);
            _tweaker25.Apply(_value, Formula.ElasticInOut);
            _tweaker26.Apply(_value, Formula.BackIn);
            _tweaker27.Apply(_value, Formula.BackOut);
            _tweaker28.Apply(_value, Formula.BackInOut);
            _tweaker29.Apply(_value, Formula.BounceIn);
            _tweaker30.Apply(_value, Formula.BounceOut);
            _tweaker31.Apply(_value, Formula.BounceInOut);
        }
    }
}