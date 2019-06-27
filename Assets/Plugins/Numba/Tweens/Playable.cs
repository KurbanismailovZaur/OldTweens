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
        #region Types
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
        #endregion

        #region Field, events and properties
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

        internal protected float _currentTime;

        internal protected Sequence _parent;

        public Sequence Parent => _parent;

        #region Playing
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
        #endregion

        public override bool keepWaiting => !IsStoped;
        #endregion

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

        private int GetLoopTypeDurationMultiplier(LoopType loopType) => loopType == LoopType.Mirror ? 2 : 1;

        protected void NormalizeTime(ref float time)
        {
            var normalizedTime = time / FullDuration;

            time = float.IsNaN(normalizedTime) ? 0f : Mathf.Clamp01(normalizedTime);
        }

        internal abstract void SetTime(float time, bool normalized = false);

        protected bool GetEvents(ref float time, bool normalized, out Events events)
        {
            if (!normalized)
                NormalizeTime(ref time);

            events = GetTimeShiftEvents(time);

            return events != null;
        }

        protected Events GetTimeShiftEvents(float time)
        {
            // Exit for both situations (when FullDuration equal or not to 0).
            if (time == _currentTime) return null;

            // If playable starts and completes immediatly it is need to just generate all events.
            if (FullDuration == 0f)
            {
                var events = new Events();

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

            return time > _currentTime ? GetForwardTimeShiftEvents(time) : GetBackwardTimeShiftEvents(time);
        }

        private Events GetForwardTimeShiftEvents(float time)
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

                if (loop > _currentTime && loop <= time)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} completed"), i - 1, Phase.LoopCompleted);

                if (loop >= _currentTime && loop < time)
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

        private Events GetBackwardTimeShiftEvents(float time)
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

                if (loop >= time && loop < _currentTime)
                    events.Add(loop, (li) => Debug.Log($"{Name} loop {li} completed"), Count - i - 1, Phase.LoopCompleted);

                if (loop > time && loop <= _currentTime)
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

        protected float WrapTime(float time, float normalizedDuration) => WrapCeil(time, normalizedDuration) / normalizedDuration;

        private float WrapCeil(float value, float max)
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

        protected float GetNormalizedDuration() => Mathf.Approximately(_duration, 0f) ? 0f : 1f / Count;

        protected float GetSelfPlayingDirection() => _loopType == LoopType.Backward ? -1f : 1f;

        protected float GetHierarchyPlayingDirection()
        {
            if (_parent == null)
                return 1f;

            return _parent.GetSelfPlayingDirection() * _parent.GetHierarchyPlayingDirection();
        }

        protected internal abstract void ResetCurrentTime();

        protected internal abstract void ResetStateAccordingToTime(float time);

        #region Playing
        protected virtual void CheckSpecificPlayExceptions() { }
        
        public Playable Play()
        {
            CheckSpecificPlayExceptions();

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

            // If we not add 1, then playables with zero duration will not called.
            SetTime(FullDuration + 1f);

            _playState = PlayState.Stop;

            ResetCurrentTime();
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

            ResetCurrentTime();

            Debug.Log("Stoped");

            return this;
        }
        #endregion
    }
}