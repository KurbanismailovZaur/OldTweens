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
        protected enum Phase : byte
        {
            Started,
            LoopStarted,
            LoopUpdated,
            LoopCompleted,
            Completed
        }

        protected class Event
        {
            public float time;

            public List<Action<int>> actions = new List<Action<int>>();

            public List<int> loopIndexes = new List<int>();

            public List<Phase> phases = new List<Phase>();

            public int Count => actions.Count;

            public Event(float time, Action<int> action, int loopIndex, Phase phase)
            {
                this.time = time;
                Add(action, loopIndex, phase);
            }

            public Event Add(Action<int> action, int loopIndex, Phase phase)
            {
                actions.Add(action);
                loopIndexes.Add(loopIndex);
                phases.Add(phase);

                return this;
            }

            public void Call(int index) => actions[index](loopIndexes[index]);

            public void CallAll()
            {
                for (int i = 0; i < actions.Count; i++)
                    Call(i);
            }
        }

        protected class Events
        {
            private List<Event> _events;

            public int Count => _events.Count;

            public Events(int capacity = 0) => _events = new List<Event>(capacity);

            public void Add(float time, Action<int> action, int loopIndex, Phase phase)
            {
                var @event = _events.Find(e => Mathf.Approximately(e.time, time));

                if (@event == null)
                    Add(new Event(time, action, loopIndex, phase));
                else
                    @event.Add(action, loopIndex, phase);
            }

            public void Add(Event @event) => _events.Add(@event);

            public Event this[int index] => _events[index];
        }

        internal protected Sequence _parent;

        public Sequence Parent => _parent;

        public string Name { get; set; }

        protected float _duration;

        public float Duration => _duration;

        protected float _fullDuration;

        public float FullDuration
        {
            get => _fullDuration;
            protected set
            {
                _fullDuration = value;
                FullDurationChanged?.Invoke(this);
            }
        }

        protected internal event Action<Playable> FullDurationChanged;

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

        internal protected float _currentTime;

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
            Name = name ?? "None";
            _duration = Mathf.Max(duration, 0f);
            Count = count;
            LoopType = loopType;

            CalculateFullDuration();
        }

        protected void ThrowChangeBusyException(string fieldName) => throw new BusyException($"Trying to change {fieldName} on playable with name \"{Name}\" when playing");

        protected void CalculateFullDuration() => FullDuration = Duration * GetLoopTypeDurationMultiplier(LoopType) * Count;

        protected int GetLoopTypeDurationMultiplier(LoopType loopType) => loopType == LoopType.Mirror ? 2 : 1;

        internal protected abstract void SetStateTo(float time);

        protected internal abstract void SetTime(float time, bool normalized = false);

        protected Events GetTimeShiftEvents(float time)
        {
            // If playable starts and completes immediatly it is need to just generate all events.
            if (FullDuration == 0f)
            {
                var events = new Events(2 + Count * 2);

                events.Add(_currentTime, (li) => Debug.Log($"{Name} started {li}"), 0, Phase.Started);
                events.Add(_currentTime, (li) => Debug.Log($"{Name} loop started {li}"), 0, Phase.LoopStarted);

                for (int i = 0; i < Count - 1; i++)
                {
                    events.Add(new Event(1f - _currentTime, (li) => Debug.Log($"{Name} loop completed {li}"), i, Phase.LoopCompleted));
                    events.Add(new Event(_currentTime, (li) => Debug.Log($"{Name} loop started {li}"), i + 1, Phase.LoopStarted));
                }

                var completeEvent = new Event(1f - _currentTime, (li) => Debug.Log($"{Name} loop completed {li}"), Count - 1, Phase.LoopCompleted);
                completeEvent.Add((li) => Debug.Log($"{Name} completed {li}"), Count - 1, Phase.Completed);

                events.Add(completeEvent);

                return events;
            }

            if (time == _currentTime) return null;

            return time > _currentTime ? GetForwardTimeShiftEvents(time) : GetReverseTimeShiftEvents(time);
        }

        protected Events GetForwardTimeShiftEvents(float time)
        {
            var events = new Events();

            // Almost the same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 0f)
                events.Add(0f, (li) => Debug.Log($"{Name} started {li}"), 0, Phase.Started);

            // Loop events.
            var currentLoop = Mathf.FloorToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.FloorToInt(time / loopDuration);

            for (int i = currentLoop; i <= nextLoop; i++)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, _currentTime, time) && loop != _currentTime)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} completed"), i - 1, Phase.LoopCompleted);

                if (IsBetween(loop, _currentTime, time) && loop != time)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} started"), i, Phase.LoopStarted);
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                events.Add(time, (li) => Debug.Log($"{Name} loop {li} updated"), nextLoop, Phase.LoopUpdated);

            // Complete event.
            if (time == 1f)
                events.Add(1f, (li) => Debug.Log($"{Name} completed {li}"), Count - 1, Phase.Completed);

            return events;
        }

        protected Events GetReverseTimeShiftEvents(float time)
        {
            var events = new Events();

            // The same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 1f)
                events.Add(1f, (li) => Debug.Log($"{Name} started {li}"), 0, Phase.Started);

            // Loop events.
            var currentLoop = Mathf.CeilToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.CeilToInt(time / loopDuration);

            for (int i = currentLoop; i >= nextLoop; i--)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, time, _currentTime) && loop != _currentTime)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} completed"), Count - i - 1, Phase.LoopCompleted);

                if (IsBetween(loop, time, _currentTime) && loop != time)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} started"), Count - i, Phase.LoopStarted);
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                events.Add(time, (li) => Debug.Log($"{Name} loop {li} updated"), Count - nextLoop, Phase.LoopUpdated);

            // Complete event.
            if (time == 0f)
                events.Add(0f, (li) => Debug.Log($"{Name} completed {li}"), Count - 1, Phase.Completed);

            return events;
        }

        protected float WrapTime(float time, float loopDuration, Phase phase)
        {
            if (time == 0f || time == 1f)
                return time;

            if (phase == Phase.Started || phase == Phase.LoopStarted)
                return 0f;

            return WrapCeil(time, loopDuration) / loopDuration;
        }

        protected float WrapCeil(float value, float max)
        {
            if (value == 0f) return 0f;

            var wrapped = value % max;
            return (wrapped == 0f) ? max : wrapped;
        }

        protected float LoopTime(float time, LoopType loopType)
        {
            switch (loopType)
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

            ResetCurrentTime(0f);
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

            ResetCurrentTime(0f);

            Debug.Log("Stoped");

            return this;
        }

        protected internal abstract void ResetCurrentTime(float time);

        protected internal abstract void ResetState(float time);
    }
}