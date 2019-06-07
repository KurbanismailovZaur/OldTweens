using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public class Tween : Playable
    {
        public Tweaker Tweaker { get; set; }

        public Formula Formula { get; set; }

        public new float Duration
        {
            get => base.Duration;
            set
            {
                if (IsBusy) ThrowChangeBusyException("duration");

                _duration = Mathf.Max(value, 0f);
                CalculateFullDuration();
            }
        }

        public Tween(Tweaker tweaker, float duration, Formula formula, int count = 1, LoopType loopType = LoopType.Forward) : base(duration, count, loopType)
        {
            Tweaker = tweaker;
            Formula = formula;
        }

        protected override void SetTime(float time, bool normalized = false)
        {
            NormalizeTime(ref time, normalized);

            if (_currentTime == time) return;

            GenerateTimeshiftEvents(time);

            _currentTime = time;

            Tweaker?.Apply(WrapCeil(time * Count, 1f), Formula);
        }

        public new Tween Play() => (Tween)base.Play();

        public new Tween Pause() => (Tween)base.Pause();

        public new Tween Stop() => (Tween)base.Stop();
    }
}