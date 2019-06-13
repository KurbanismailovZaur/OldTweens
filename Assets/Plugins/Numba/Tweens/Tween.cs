﻿using System.Collections;
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

        internal protected override List<Tweaker> Tweakers => new List<Tweaker> { Tweaker };

        public Tween(Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : this(null, tweaker, duration, formula, count, loopType) { }

        public Tween(string name, Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : base(name, duration, count, loopType)
        {
            Tweaker = tweaker;
            Formula = formula;
        }

        protected internal override void SetTime(float time, bool normalized = false)
        {
            var events = GetTimeShiftEvents(ref time, normalized);

            if (events != null)
            {
                var loopDuration = Mathf.Approximately(_duration, 0f) ? 0f : 1f / Count;

                for (int i = 0; i < events.Count; i++)
                {
                    _currentTime = events[i].time;

                    Tweaker?.Apply(LoopTime(events[i].time, loopDuration), Formula);
                    events[i].Call();
                }
            }
        }

        public void SetTimeIIIIUUUHH(float time) => SetTime(time, true);

        public new Tween Play() => (Tween)base.Play();

        public new Tween Pause() => (Tween)base.Pause();

        public new Tween Stop() => (Tween)base.Stop();
    }
}