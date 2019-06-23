using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public sealed class Tween : Playable
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

        public Tween(Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : this(null, tweaker, duration, formula, count, loopType) { }

        public Tween(string name, Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : base(name, duration, count, loopType)
        {
            Tweaker = tweaker;
            Formula = formula;
        }

        internal override void SetTime(float time, bool normalized = false)
        {
            if (!GetEvents(ref time, normalized, out Events events))
                return;

            int startIndex = 0;

            // Calling start and loop start events.
            if (events[0].phases[0] == Phase.Started)
            {
                // WrapTime not needed, because _current time will be equal to 0 or 1.
                Tweaker?.Apply(LoopTime(events[0].time, _loopType), Formula);
                events[0].CallAll();

                startIndex = 1;
            }

            var normalizedDuration = GetNormalizedDuration();
            float wrappedTime;

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                // It is not requared to save intermediate current time values, 
                // but may be useful when exception was throwed in tweaker.
                _currentTime = events[i].time;

                for (int j = 0; j < events[i].Count; j++)
                {
                    wrappedTime = WrapTime(events[i].time, normalizedDuration, events[i].phases[j]);

                    Tweaker?.Apply(LoopTime(wrappedTime, _loopType), Formula);
                    events[i].Call(j);
                }
            }

            // Save last current time value, again for exceptions in tweaker.
            _currentTime = events[events.Count - 1].time;

            if (Mathf.Approximately(normalizedDuration, 0f))
                wrappedTime = events[events.Count - 1].time;
            else
                wrappedTime = WrapTime(events[events.Count - 1].time, normalizedDuration);

            // Calling update or complete and loop complete events.
            Tweaker?.Apply(LoopTime(wrappedTime, _loopType), Formula);
            events[events.Count - 1].CallAll();
        }

        public void SetTimeIIIIUUUHH(float time) => SetTime(time);
    }
}