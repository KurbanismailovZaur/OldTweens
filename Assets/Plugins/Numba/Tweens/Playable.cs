using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public abstract class Playable
    {
        public class BusyException : ApplicationException
        {
            public BusyException(string message) : base(message) { }
        }

        internal Playable _parent;

        public Playable Parent => _parent;

        public string Name { get; set; }

        protected float _duration;

        public float Duration => _duration;

        protected int _count;

        public int Count
        {
            get => _count;
            set
            {
                if (IsPlaying) ThrowBusyException("count");

                _count = Mathf.Max(value, 0);
                CalculateFullDuration();
            }
        }

        protected LoopType _loopType;

        public LoopType LoopType
        {
            get => _loopType;
            set
            {
                if (IsPlaying) ThrowBusyException("loop type");

                _loopType = Enum.IsDefined(typeof(LoopType), value) ? value : throw new ArgumentException("Loop type must be forward, backward or mirror");
                CalculateFullDuration();
            }
        }

        protected float _currentTime;

        private bool _isPlaying;

        public bool IsPlaying => _isPlaying || (_parent?.IsPlaying ?? false);

        public float FullDuration { get; protected set; }

        public Playable(float duration, int count = 1, LoopType loopType = LoopType.Forward) : this(null, duration, count, loopType) { }

        public Playable(string name, float duration, int count = 1, LoopType loopType = LoopType.Forward)
        {
            Name = name;
            _duration = Mathf.Max(duration, 0f);
            Count = count;
            LoopType = loopType;

            CalculateFullDuration();
        }

        protected void ThrowBusyException(string fieldName) => throw new BusyException($"Trying to change {fieldName} on playable with name \"{Name}\" when playing");

        protected void CalculateFullDuration() => FullDuration = Duration * GetLoopTypeDurationMultiplier(LoopType) * Count;

        private int GetLoopTypeDurationMultiplier(LoopType loopType) => loopType == LoopType.Mirror ? 2 : 1;

        protected abstract void SetTime(float time, bool normalized = false);

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
            if (nextTime != nextLoop)
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
    }
}