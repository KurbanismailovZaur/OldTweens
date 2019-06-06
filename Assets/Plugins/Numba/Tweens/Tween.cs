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
                if (IsPlaying) ThrowBusyException("duration");

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
            if (!normalized)
                time = time / FullDuration;
				
			time = Mathf.Clamp01(time);

            if (_currentTime == time) return;

            GenerateTimeshiftEvents(time);

            _currentTime = time;

            Tweaker.Apply(time, Formula);
        }

        public void SetTime1(float time, bool normalized) => SetTime(time, normalized);
    }
}