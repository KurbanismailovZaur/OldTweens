using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public class Sequence : Playable
    {
        private Phase _currentTimePhase;

        protected List<(int order, float time, Playable playable)> _playables = new List<(int order, float, Playable)>();

        protected int _nextOrder;

        public Sequence(int count = 1, LoopType loopType = LoopType.Forward) : this(null, count, loopType) { }

        public Sequence(string name, int count = 1, LoopType loopType = LoopType.Forward) : base(name, 0f, count, loopType) { }

        protected internal override List<Tweaker> Tweakers => throw new NotImplementedException();

        internal protected override void SetStateTo(float time)
        {
            // for (int i = 0; i < _playables; i++)
            // {

            // }
        }

        protected void SetCurrentTime(float time, Phase phase)
        {
            _currentTime = time;
            _currentTimePhase = phase;
        }

        protected internal override void SetTime(float time, bool normalized = false)
        {
            if (!normalized)
                time = Mathf.Clamp01(time / FullDuration);

            var events = GetTimeShiftEvents(time);

            if (events == null)
                return;

            var loopDuration = Mathf.Approximately(_duration, 0f) ? 0f : 1f / Count;
            var playableStartTime = GetLoopedStartTime();
            int startIndex = 0;

            // Calling start and loop start events.
            // No one playable will be called, because any playable start when time != 0.
            if (events[0].phases[0] == Phase.Started)
            {
                SetCurrentTime(events[0].time, Phase.LoopStarted);

                events[0].CallAll();
                startIndex += 1;
            }

            // Times in seconds.
            float currentTime;
            float nextTime;

            List<(int order, float time, Playable playable)> playables;

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                for (int j = 0; j < events[i].Count; j++)
                {
                    // Reset state to correct handle time events on playables.
                    if (events[i].phases[j] == Phase.LoopStarted)
                    {
                        _currentTime = 0f;

                        for (int k = 0; k < _playables.Count; k++)
                            _playables[k].playable._currentTime = playableStartTime;
                    }

                    currentTime = LoopTime(WrapTime(_currentTime, loopDuration, events[i].phases[j])) * _duration;
                    nextTime = LoopTime(WrapTime(events[i].time, loopDuration, events[i].phases[j])) * _duration;

                    playables = GetCurrentPlayables(currentTime, nextTime);

                    for (int k = 0; k < playables.Count; k++)
                        playables[k].playable.SetTime(nextTime - playables[k].time);

                    SetCurrentTime(events[i].time, events[i].phases[j]);

                    events[i].Call(j);
                }
            }

            // Calling update or complete and loop complete events.
            currentTime = LoopTime(WrapTime(_currentTime, loopDuration, _currentTimePhase)) * _duration;
            nextTime = LoopTime(WrapTime(events[events.Count - 1].time, loopDuration, events[events.Count - 1].phases[0] == Phase.LoopUpdated ? Phase.LoopUpdated : Phase.Completed)) * _duration;

            playables = GetCurrentPlayables(currentTime, nextTime);

            for (int k = 0; k < playables.Count; k++)
                playables[k].playable.SetTime(nextTime - playables[k].time);

            SetCurrentTime(events[events.Count - 1].time, events[events.Count - 1].phases[events[events.Count - 1].phases.Count - 1]);
            events[events.Count - 1].CallAll();
        }

        protected float GetLoopedStartTime() => _loopType == LoopType.Backward ? 1f : 0f;

        protected List<(int order, float time, Playable playable)> GetCurrentPlayables(float currentTime, float time)
        {
            return currentTime <= time ? GetCurrentForwardPlayables(currentTime, time) : GetCurrentBackwardPlayables(currentTime, time);
        }

        protected List<(int order, float time, Playable playable)> GetCurrentForwardPlayables(float currentTime, float time)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time + _playables[i].playable.FullDuration >= currentTime && _playables[i].time < time)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order));

            // Add last playables which can't be started later.
            if (time == FullDuration)
            {
                var lastPlayables = new List<(int order, float time, Playable playable)>();

                for (int i = 0; i < _playables.Count; i++)
                {
                    if (_playables[i].time == FullDuration)
                        lastPlayables.Add(_playables[i]);
                }

                lastPlayables.Sort((a, b) => a.order.CompareTo(b.order));

                playables.AddRange(lastPlayables);
            }

            return playables;
        }

        protected List<(int order, float time, Playable playable)> GetCurrentBackwardPlayables(float currentTime, float time)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time <= currentTime && _playables[i].time + _playables[i].playable.FullDuration > time)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order));

            // Add last playables which can't be started later.
            if (time == 0f)
            {
                var lastPlayables = new List<(int order, float time, Playable playable)>();

                for (int i = 0; i < _playables.Count; i++)
                {
                    if (_playables[i].time + _playables[i].playable.FullDuration == 0f)
                        lastPlayables.Add(_playables[i]);
                }

                lastPlayables.Sort((a, b) => a.order.CompareTo(b.order));

                playables.AddRange(lastPlayables);
            }

            return playables;
        }

        public void SetTimeIIIUUUHH(float time) => SetTime(time, true);

        protected bool CheckOnCyclicReference(Sequence sequence)
        {
            if (sequence == this)
                return true;

            for (int i = 0; i < sequence._playables.Count; i++)
            {
                if (!(sequence._playables[i].playable is Sequence subSequence))
                    continue;

                if (CheckOnCyclicReference(subSequence))
                    return true;
            }

            return false;
        }

        public void Append(Playable playable) => Append(playable, _nextOrder++);

        public void Append(Playable playable, int order) => Insert(_duration, playable, order);

        public void Insert(float time, Playable playable) => Insert(time, playable, _nextOrder++);

        public void Insert(float time, Playable playable, int order)
        {
            if (playable == this)
                throw new ArgumentException($"Sequence \"{Name}\" can't contain itself");

            if (playable is Sequence sequence && CheckOnCyclicReference(sequence))
                throw new ArgumentException($"Cyclic references detected. Sequence \"{playable.Name}\" or its child sequences (in whole hierarchy) already referenced to sequence \"{Name}\"");

            if (_playables.FindIndex(p => p.playable == playable) != -1)
                throw new ArgumentException($"Sequence \"{Name}\" already contains playable with name \"{playable.Name}\"");

            order = Mathf.Max(order, 0);

            var playables = _playables.FindAll(p => p.order >= order);

            for (int i = 0; i < playables.Count; i++)
            {
                var p = playables[i];
                p.order += 1;
                playables[i] = p;
            }

            _playables.Add((order, Mathf.Max(time, 0f), playable));
            playable._parent = this;

            playable.FullDurationChanged += Playable_FullDurationChanged;

            _duration = Mathf.Max(time + playable.FullDuration, _duration);
            CalculateFullDuration();
        }

        protected void CalculateDuration()
        {
            _duration = 0f;

            for (int i = 0; i < _playables.Count; i++)
                _duration = Mathf.Max(_duration, _playables[i].time + _playables[i].playable.FullDuration);
        }

        protected void CalculateAllDurations()
        {
            CalculateDuration();
            CalculateFullDuration();
        }

        public new Sequence Play() => (Sequence)base.Play();

        public new Sequence Pause() => (Sequence)base.Pause();

        public new Sequence Stop() => (Sequence)base.Stop();

        #region Event handlers
        protected void Playable_FullDurationChanged(Playable playable) => CalculateAllDurations();
        #endregion
    }
}