using UnityEngine;
using System;

namespace Numba.Tweens
{
    public abstract class Tween : Playable
    {
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

        public Tween(float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : this(null, duration, formula, count, loopType) { }

        public Tween(string name, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : base(name, duration, count, loopType) => Formula = formula;

        public void SetTimeAAAAAAA(float time) => SetTime(time);

        protected internal override void ResetCurrentTime() => _currentTime = 0f;

        public new Tween Play() => (Tween)base.Play();

        public new Tween Pause() => (Tween)base.Pause();

        public new Tween Stop() => (Tween)base.Stop();
    }

    public sealed class Tween<T> : Tween where T : struct
    {
        public Func<T> From { get; set; }

        public Func<T> To { get; set; }

        public Action<T> Setter;

        public Tweak<T> Tweak { get; set; }

        public Tween(Func<T> from, Func<T> to, Action<T> setter, Tweak<T> tweak, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : this(null, from, to, setter, tweak, duration, formula, count, loopType) { }

        public Tween(string name, Func<T> from, Func<T> to, Action<T> setter, Tweak<T> tweak, float duration, Formula formula = null, int count = 1, LoopType loopType = LoopType.Repeat) : base(name, duration, formula, count, loopType)
        {
            From = from;
            To = to;
            Setter = setter;
            Tweak = tweak;
        }

        protected internal override void SetTime(float time, bool normalized = false)
        {
            if (Mathf.Approximately(FullDuration, 0f))
                SetTimeWhenDurationIsZero(time, normalized);
            else
                SetTimeWhenDurationIsNotZero(time, normalized);
        }

        private void SetTimeWhenDurationIsZero(float time, bool normalized)
        {
            if (!GetEvents(ref time, normalized, out Events events))
                return;

            int startIndex = 0;

            var isForward = _currentTime < time;

            var from = default(T);
            var to = default(T);

            // Calling start and loop start events.
            if (events[0].phases[0] == Phase.Started)
            {
                from = From();
                to = To();

                if (_loopType == LoopType.Increment)
                {
                    var pair = (isForward ? 0 : events.Count - 1) / 2;
                    (from, to) = (Tweak?.Evaluate(from, to, pair) ?? from, Tweak?.Evaluate(from, to, pair + 1) ?? to);
                }

                // WrapTime not needed, because _current time will be equal to 0 or 1.
                Tweak?.Apply(from, to, LoopTime(events[0].time, _loopType), Setter, Formula);
                events[0].CallAll();

                startIndex = 1;
            }

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                for (int j = 0; j < events[i].Count; j++)
                {
                    // It is not requared to save intermediate current time values, 
                    // but may be useful when exception was throwed in tweaker.
                    _currentTime = events[i].time;

                    from = From();
                    to = To();

                    if (_loopType == LoopType.Increment)
                    {
                        var pair = (isForward ? i : events.Count - 1 - i) / 2;
                        (from, to) = (Tweak?.Evaluate(from, to, pair) ?? from, Tweak?.Evaluate(from, to, pair + 1) ?? to);
                    }

                    Tweak?.Apply(from, to, LoopTime(events[i].time, _loopType), Setter, Formula);
                    events[i].Call(j);
                }
            }

            // Save last current time value, again for exceptions in tweaker.
            _currentTime = events[events.Count - 1].time;

            from = From();
            to = To();

            if (_loopType == LoopType.Increment)
            {
                var pair = (isForward ? events.Count - 1 : 0) / 2;
                (from, to) = (Tweak?.Evaluate(from, to, pair) ?? from, Tweak?.Evaluate(from, to, pair + 1) ?? to);
            }

            // Calling update or complete and loop complete events.
            Tweak?.Apply(from, to, LoopTime(events[events.Count - 1].time, _loopType), Setter, Formula);
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
                Tweak?.Apply(From(), To(), LoopTime(events[0].time, _loopType), Setter, Formula);
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

                    Tweak?.Apply(From(), To(), LoopTime(wrappedTime, _loopType), Setter, Formula);
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
            Tweak?.Apply(From(), To(), LoopTime(wrappedTime, _loopType), Setter, Formula);
            events[events.Count - 1].CallAll();
        }

        protected internal override void ResetStateAccordingToTime(float time) => Tweak?.Apply(From(), To(), LoopTime(time, _loopType), Setter, Formula);
    }
}