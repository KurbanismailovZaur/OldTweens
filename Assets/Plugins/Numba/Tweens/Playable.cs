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
                _currentTime = _loopType == LoopType.Backward ? 1f : 0f;

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

        protected bool GenerateTimeshiftEvents(ref float time, bool normalized)
        {
            if (!normalized) 
                time = Mathf.Clamp01(time / FullDuration);

            if (time == _currentTime) return false;

            if (_loopType == LoopType.Forward)
            {
                if (time > _currentTime)
                    GenerateForwardTimeshiftEvents(ref time);
                else 
                    GenerateForwardReverseTimeshiftEvents(ref time);
            }

            return true;
        }

        protected void GenerateForwardTimeshiftEvents(ref float time)
        {
            // Almost the same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 0f)
                Debug.Log("Started");

            // Loop events.
            var currentLoop = Mathf.FloorToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.FloorToInt(time / loopDuration);

            for (int i = currentLoop; i <= nextLoop; i++)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, _currentTime, time) && loop != _currentTime)
                    Debug.Log("Loop completed");

                if (IsBetween(loop, _currentTime, time) && loop != time)
                    Debug.Log("Loop started");
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                Debug.Log("Updated");

            // Complete event.
            if (time == 1f)
                Debug.Log("Completed");

            _currentTime = time;

            // To save calculation accuracy it is important to leave if time equals to zero or one 
            if (time == 0f || time == 1f) return;

            time = WrapCeil(time, loopDuration);
            time /= loopDuration;
        }

        protected void GenerateForwardReverseTimeshiftEvents(ref float time)
        {
            // The same thing as Duration / FullDuration.
            var loopDuration = 1f / Count;

            // Start event.
            if (_currentTime == 1f)
                Debug.Log("Started");

            // Loop events.
            var currentLoop = Mathf.CeilToInt(_currentTime / loopDuration);
            var nextLoop = Mathf.CeilToInt(time / loopDuration);

            for (int i = currentLoop; i >= nextLoop; i--)
            {
                var loop = loopDuration * i;

                if (IsBetween(loop, time, _currentTime) && loop != _currentTime)
                    Debug.Log("Loop completed");

                if (IsBetween(loop, time, _currentTime) && loop != time)
                    Debug.Log("Loop started");
            }

            // Update event.
            if (!Mathf.Approximately(time, loopDuration * nextLoop))
                Debug.Log("Updated");

            // Complete event.
            if (time == 0f)
                Debug.Log("Completed");

            _currentTime = time;

            // To save calculation accuracy it is important to leave if time equals to zero or one 
            if (time == 0f || time == 1f) return;

            time = WrapCeil(time, loopDuration);
            time /= loopDuration;
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
                var time = (Time.time - _startTime) / FullDuration;

                SetTime(time);

                yield return null;
            }

            SetTime(1f);

            _playState = PlayState.Stop;
        }

        // protected void SetMirrorTime(float time)
        // {
        //     var ceiled = WrapCeil(time * 2f, 1f);

        //     bool isForwardSubLoop = true;

        //     if (time != 0f)
        //     {
        //         var halfLoopDuration = 1f / (Count * 2);
        //         var currentSubLoop = time / halfLoopDuration;

        //         isForwardSubLoop = ((int)currentSubLoop % 2 == 0 && currentSubLoop != (int)currentSubLoop) || ((int)currentSubLoop % 2 != 0 && currentSubLoop == (int)currentSubLoop) ? true : false;
        //     }

        //     _allowStartEvent = _currentTime == 0f;
        //     _allowCompleteEvent = time == 1f;

        //     SetTime(isForwardSubLoop ? ceiled : 1f - ceiled, true);

        //     _allowStartEvent = _allowCompleteEvent = true;
        // }

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

            _currentTime = _loopType == LoopType.Backward ? 1f : 0f;

            Debug.Log("Stoped");

            return this;
        }
    }
}