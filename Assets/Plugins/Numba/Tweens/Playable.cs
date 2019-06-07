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
        internal Playable _parent;

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

        private PlayState _playState = PlayState.Stop;

        public PlayState PlayState => _playState;

        public bool IsPlaying => _playState == PlayState.Play;

        public bool IsPaused => _playState == PlayState.Pause;

        public bool IsStoped => _playState == PlayState.Stop;

        public bool IsBusy => !IsStoped || (_parent?.IsBusy ?? false);

        private IEnumerator _playEnumerator;

        private Coroutine _playCoroutine;

        private float _startTime;

        private float _endTime;

        private float _pauseTime;

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

        private int GetLoopTypeDurationMultiplier(LoopType loopType) => loopType == LoopType.Mirror ? 2 : 1;

        internal protected abstract void SetTime(float time, bool normalized = false);

        protected void NormalizeTime(ref float time, bool normalized)
        {
            if (!normalized)
                time = time / FullDuration;

            time = Mathf.Clamp01(time);
        }

        protected void GenerateTimeshiftEvents(float nextTime)
        {
            if (nextTime > _currentTime)
                GenerateForwardTimeshiftEvents(nextTime);
            else
                GenerateBackwardTimeshiftEvents(nextTime);
        }

        private void GenerateForwardTimeshiftEvents(float nextTime)
        {
            // The same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 0f)
                Debug.Log("Started");

            // Loop events.
            var currentLoop = (int)(_currentTime / loopDuration);
            var nextLoop = (int)(nextTime / loopDuration);

            for (int i = currentLoop; i <= nextLoop; i++)
            {
                if (IsBetween(loopDuration * i, _currentTime, nextTime))
                {
                    if (i != 0)
                        Debug.Log("Loop completed");

                    if (i != Count)
                        Debug.Log("Loop started");
                }
            }

            // Update event.
            if (nextTime != loopDuration * nextLoop)
                Debug.Log("Updated");

            // Complete event.
            if (nextTime == 1f)
                Debug.Log("Completed");
        }

        private void GenerateBackwardTimeshiftEvents(float nextTime)
        {
            // The same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 1f)
                Debug.Log("Started");

            // Loop events.
            var currentLoop = (int)(_currentTime / loopDuration);
            var nextLoop = (int)(nextTime / loopDuration);

            for (int i = currentLoop; i >= nextLoop; i--)
            {
                if (IsBetween(loopDuration * i, nextTime, _currentTime))
                {
                    if (i != Count)
                        Debug.Log("Loop completed");

                    if (i != 0)
                        Debug.Log("Loop started");
                }
            }

            // Update event.
            if (nextTime != nextLoop)
                Debug.Log("Updated");

            // Complete event.
            if (nextTime == 0f)
                Debug.Log("Completed");
        }

        private bool IsBetween(float value, float min, float max) => value >= min && value <= max;

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

        private IEnumerator PlayEnumerator()
        {
            _currentTime = 0f;

            _startTime = Time.time;
            _endTime = _startTime + FullDuration;

            while (Time.time < _endTime)
            {
                SetTime(GetNormalizedLoopedTime(Time.time - _startTime), true);
                yield return null;
            }

            SetTime(GetNormalizedLoopedTime(FullDuration), true);

            _playState = PlayState.Stop;
        }

        private float GetNormalizedLoopedTime(float time)
        {
            time /= FullDuration;
            return _loopType == LoopType.Forward ? time : _loopType == LoopType.Backward ? 1f - time : WrapCeil(time * 2f, 1f);
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

            Debug.Log("Stoped");

            return this;
        }
    }
}