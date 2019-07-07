using UnityEngine;

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

        public Tween(Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : this(null, tweaker, duration, formula, count, loopType) { }

        public Tween(string name, Tweaker tweaker, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : base(name, duration, count, loopType)
        {
            Tweaker = tweaker;
            Formula = formula;
        }

        protected internal override void SetTime(float time, bool normalized = false)
        {
            if (Mathf.Approximately(FullDuration, 0f))
                SetTimeWhenDurationIsZero(time, normalized);
            else
                SetTimeWhenDurationIsNotZero(time, normalized);
        }

        public void SetTimeAAAAAAA(float time) => SetTime(time);

        private void SetTimeWhenDurationIsZero(float time, bool normalized)
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

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                for (int j = 0; j < events[i].Count; j++)
                {
                    // It is not requared to save intermediate current time values, 
                    // but may be useful when exception was throwed in tweaker.
                    _currentTime = events[i].time;

                    Tweaker?.Apply(LoopTime(events[i].time, _loopType), Formula);
                    events[i].Call(j);
                }
            }

            // Save last current time value, again for exceptions in tweaker.
            _currentTime = events[events.Count - 1].time;

            // Calling update or complete and loop complete events.
            Tweaker?.Apply(LoopTime(events[events.Count - 1].time, _loopType), Formula);
            events[events.Count - 1].CallAll();
        }

        private void SetTimeWhenDurationIsNotZero(float time, bool normalized)
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
            var playingStartTime = _currentTime < time ? 0f : 1f;
            float wrappedTime;

            // Calling loop complete and loop start events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                for (int j = 0; j < events[i].Count; j++)
                {
                    // It is not requared to save intermediate current time values, 
                    // but may be useful when exception was throwed in tweaker.
                    _currentTime = events[i].time;

                    if (events[i].phases[j] == Phase.LoopStarted)
                        // When loop start events detected it means, 
                        // that wrapped time must be 0 in forward direction and 1 in backward.
                        wrappedTime = playingStartTime;
                    else
                        // After we wrap time it is need to substract playing start time from it,
                        // because in backward direction loop complete events must be wraped as zero.
                        wrappedTime = WrapTime(events[i].time, normalizedDuration) - playingStartTime;

                    Tweaker?.Apply(LoopTime(wrappedTime, _loopType), Formula);
                    events[i].Call(j);
                }
            }

            // Save last current time value, again for exceptions in tweaker.
            _currentTime = events[events.Count - 1].time;

            wrappedTime = WrapTime(events[events.Count - 1].time, normalizedDuration);

            // when we stopped on loop completed event state it is need substract playing start time again,
            // otherwise tweaker will take wrong parameter value.
            if (events[events.Count - 1].Count == 1 && events[events.Count - 1].phases[0] == Phase.LoopCompleted)
                wrappedTime -= playingStartTime;

            // Calling update, loop complete or loop complete + complete events.
            Tweaker?.Apply(LoopTime(wrappedTime, _loopType), Formula);
            events[events.Count - 1].CallAll();
        }

        protected internal override void ResetCurrentTime() => _currentTime = 0f;

        protected internal override void ResetStateAccordingToTime(float time) => Tweaker?.Apply(LoopTime(time, _loopType), Formula);

        public new Tween Play() => (Tween)base.Play();

        public new Tween Pause() => (Tween)base.Pause();

        public new Tween Stop() => (Tween)base.Stop();
    }
}