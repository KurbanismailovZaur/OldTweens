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

        internal protected override List<Tweaker> Tweakers => new List<Tweaker> { Tweaker };

        public Tween(Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : this(null, tweaker, duration, formula, count, loopType) { }

        public Tween(string name, Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Forward) : base(name, duration, count, loopType)
        {
            Tweaker = tweaker;
            Formula = formula;
        }

        internal protected override void SetStateTo(float time) => Tweaker?.Apply(LoopTime(time, _loopType), Formula);

        protected internal override void SetTime(float time, bool normalized = false)
        {
            if (!normalized)
                time = Mathf.Clamp01(time / FullDuration);

            // It is normal to get events when time == 0 even if Backward mode is turned on.
            var events = GetTimeShiftEvents(time);

            if (events == null)
                return;

            var loopDuration = Mathf.Approximately(_duration, 0f) ? 0f : 1f / Count;
            int startIndex = 0;

            // Calling start and loop start events.
            if (events[0].phases[0] == Phase.Started)
            {
                _currentTime = 0f;

                Tweaker?.Apply(LoopTime(events[0].time, _loopType), Formula);
                events[0].CallAll();

                startIndex += 1;
            }

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                _currentTime = events[i].time;

                for (int j = 0; j < events[i].Count; j++)
                {
                    Tweaker?.Apply(LoopTime(WrapTime(events[i].time, loopDuration, events[i].phases[j]), _loopType), Formula);
                    events[i].Call(j);
                }
            }

            _currentTime = events[events.Count - 1].time;

            // Calling update or complete and loop complete events.
            Tweaker?.Apply(LoopTime(WrapTime(events[events.Count - 1].time, loopDuration, events[events.Count - 1].phases[0] == Phase.LoopUpdated ? Phase.LoopUpdated : Phase.LoopCompleted), _loopType), Formula);
            events[events.Count - 1].CallAll();
        }

        protected internal override void ResetCurrentTime(float time) => _currentTime = time;

        protected internal override void ResetState(float time)
        {
            ResetCurrentTime(time);
            Tweaker?.Apply(time, Formula);
        }

        public void SetTimeIIIIUUUHH(float time) => SetTime(time, true);
    }
}