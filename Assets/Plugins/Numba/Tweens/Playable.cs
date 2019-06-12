using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Exceptions;

namespace Numba.Tweens
{
    public abstract class Playable : CustomYieldInstruction
    {
        protected struct Event
        {
            public float time;

            public Action<int> action;

            public int loopIndex;

            public Event(float time, Action<int> action, int loopIndex)
            {
                this.time = time;
                this.action = action;
                this.loopIndex = loopIndex;
            }

            public void Call() => action(loopIndex);
        }

        internal protected Playable _parent;

        public Playable Parent => _parent;

        public string Name { get; set; } = "None";

        protected float _duration;

        public float Duration => _duration;

        public float FullDuration { get; protected set; }

        protected int _count;

        public int Count
        {
            get => _count;
            set
            {
                if (IsBusy) ThrowChangeBusyException("count");

                _count = Mathf.Max(value, 1);
                CalculateFullDuration();
            }
        }

        protected LoopType _loopType;

        public LoopType LoopType
        {
            get => _loopType;
            set
            {
                if (IsBusy) ThrowChangeBusyException("loop type");

                _loopType = Enum.IsDefined(typeof(LoopType), value) ? value : throw new ArgumentException("Loop type must be forward, backward or mirror");

                CalculateFullDuration();
            }
        }

        internal protected abstract List<Tweaker> Tweakers { get; }

        protected float _currentTime;

        protected PlayState _playState = PlayState.Stop;

        public PlayState PlayState => _playState;

        public bool IsPlaying => _playState == PlayState.Play;

        public bool IsPaused => _playState == PlayState.Pause;

        public bool IsStoped => _playState == PlayState.Stop;

        public bool IsBusy => !IsStoped || (_parent?.IsBusy ?? false);

        protected IEnumerator _playEnumerator;

        protected Coroutine _playCoroutine;

        protected float _startTime;

        protected float _endTime;

        protected float _pauseTime;

        public override bool keepWaiting => !IsStoped;

        public Playable(float duration, int count = 1, LoopType loopType = LoopType.Forward) : this(null, duration, count, loopType) { }

        public Playable(string name, float duration, int count = 1, LoopType loopType = LoopType.Forward)
        {
            Name = name;
            _duration = Mathf.Max(duration, 0f);
            Count = count;
            LoopType = loopType;

            CalculateFullDuration();
        }

        protected void ThrowChangeBusyException(string fieldName) => throw new BusyException($"Trying to change {fieldName} on playable with name \"{Name}\" when playing");

        protected void CalculateFullDuration() => FullDuration = Duration * GetLoopTypeDurationMultiplier(LoopType) * Count;

        protected int GetLoopTypeDurationMultiplier(LoopType loopType) => loopType == LoopType.Mirror ? 2 : 1;

        protected abstract void SetTime(float time, bool normalized = false);

        protected List<Event> GetTimeShiftEvents(float time, bool normalized)
        {
            if (!normalized)
                time = Mathf.Clamp01(time / FullDuration);

            if (time == _currentTime) return null;

            return time > _currentTime ? GetTimeShiftEvents(time) : GetReverseTimeShiftEvents(time);
        }

        protected List<Event> GetTimeShiftEvents(float time)
        {
            var events = new List<Event>();

            // Almost the same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 0f)
                events.Add(new Event(WrapAndLoopTime(0f, loopDuration), (li) => Debug.Log($"Started {li}"), 0));

            // Loop events.
            var currentLoop = Mathf.FloorToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.FloorToInt(time / loopDuration);

            for (int i = currentLoop; i <= nextLoop; i++)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, _currentTime, time) && loop != _currentTime)
                    events.Add(new Event(WrapAndLoopTime(loop, loopDuration), (li) => Debug.Log($"Loop {li} completed"), i - 1));

                if (IsBetween(loop, _currentTime, time) && loop != time)
                    events.Add(new Event(WrapAndLoopTime(loop, loopDuration), (li) => Debug.Log($"Loop {li} started"), i));
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                events.Add(new Event(WrapAndLoopTime(time, loopDuration), (li) => Debug.Log($"Loop {li} updated"), nextLoop));

            // Complete event.
            if (time == 1f)
                events.Add(new Event(WrapAndLoopTime(1f, loopDuration), (li) => Debug.Log($"Completed {li}"), Count - 1));

            _currentTime = time;

            return events;
        }

        protected List<Event> GetReverseTimeShiftEvents(float time)
        {
            var events = new List<Event>();

            // The same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 1f)
                events.Add(new Event(WrapAndLoopTime(1f, loopDuration), (li) => Debug.Log($"Started {li}"), 0));

            // Loop events.
            var currentLoop = Mathf.CeilToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.CeilToInt(time / loopDuration);

            for (int i = currentLoop; i >= nextLoop; i--)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, time, _currentTime) && loop != _currentTime)
                    events.Add(new Event(WrapAndLoopTime(loop, loopDuration), (li) => Debug.Log($"Loop {li} completed"), Count - i - 1));

                if (IsBetween(loop, time, _currentTime) && loop != time)
                    events.Add(new Event(WrapAndLoopTime(loop, loopDuration), (li) => Debug.Log($"Loop {li} started"), Count - i));
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                events.Add(new Event(WrapAndLoopTime(time, loopDuration), (li) => Debug.Log($"Loop {li} updated"), Count - nextLoop));

            // Complete event.
            if (time == 0f)
                events.Add(new Event(WrapAndLoopTime(1f, loopDuration), (li) => Debug.Log($"Completed {li}"), Count - 1));

            _currentTime = time;

            return events;
        }

        protected float WrapAndLoopTime(float time, float loopDuration)
        {
            if (time != 0f && time != 1f)
            {
                time = WrapCeil(time, loopDuration);
                time /= loopDuration;
            }

            switch (_loopType)
            {
                case LoopType.Forward:
                    return time;
                case LoopType.Backward:
                    return 1f - time;
                case LoopType.Mirror:
                    return time <= 0.5f ? time * 2f : (1f - time) * 2f;
                default:
                    throw new Exception("Loop type must be a valid value");
            }
        }
        protected bool IsBetween(float value, float min, float max) => value >= min && value <= max;

        protected float WrapCeil(float value, float max)
        {
            if (value == 0f) return 0f;

            var wrapped = value % max;
            return (wrapped == 0f) ? max : wrapped;
        }

        public Playable Play()
        {
            if (IsPlaying || (_parent?.IsBusy ?? false)) throw new BusyException($"Playable with name \"{Name}\" already playing");

            if (IsStoped)
            {
                _playState = PlayState.Play;
                _playEnumerator = PlayEnumerator();
            }
            else
            {
                var pauseDuration = Time.time - _pauseTime;

                _startTime += pauseDuration;
                _endTime += pauseDuration;

                Debug.Log("Resumed");
            }

            _playCoroutine = CoroutineHelper.Instance.StartCoroutine(_playEnumerator);

            return this;
        }

        protected IEnumerator PlayEnumerator()
        {
            _startTime = Time.time;
            _endTime = _startTime + FullDuration;

            while (Time.time < _endTime)
            {
                var time = (Time.time - _startTime);

                SetTime(time);

                yield return null;
            }

            SetTime(FullDuration);

            _playState = PlayState.Stop;
            _currentTime = 0f;
        }

        public Playable Pause()
        {
            if (!IsPlaying) return this;

            CoroutineHelper.Instance.StopCoroutine(_playCoroutine);

            _pauseTime = Time.time;

            _playState = PlayState.Pause;

            Debug.Log("Paused");

            return this;
        }

        public Playable Stop()
        {
            if (IsStoped) return this;

            CoroutineHelper.Instance.StopCoroutine(_playCoroutine);
            _playState = PlayState.Stop;

            _currentTime = 0f;

            Debug.Log("Stoped");

            return this;
        }
    }
}