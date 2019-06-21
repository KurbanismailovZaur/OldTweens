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

            // It is normal to get events when time == 0 even if Backward mode is turned on.
            var events = GetTimeShiftEvents(time);

            if (events == null)
                return;

            var playablesStartTime = Mathf.Approximately(GetLoopedStartTime(), 1f) ? 0f : 1f;

            // Reset all tweens to be ready for forward or backward playing.
            // Nested sequences in some cases played from 1 to 0, and therefore in this cases it is need to compare start time with 1.
            // To detect with what start time we need compare to initialize we just need get looped start time from parent or use 0 if it is not exist.
            var startTime = Mathf.Approximately(_parent?.GetLoopedStartTime() ?? 0f, -1f) ? 1f : 0f;

            if (Mathf.Approximately(_currentTime, startTime))
            {
                for (int k = 0; k < _playables.Count; k++)
                    _playables[k].playable.ResetCurrentTime(playablesStartTime);
            }

            var loopDuration = Mathf.Approximately(_duration, 0f) ? 0f : 1f / Count;

            // Used in mirror mode.
            var subloopDuration = loopDuration / 2f;

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

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                CheckAndSetMirrorHalfTime(loopDuration, subloopDuration, events[i].time);

                for (int j = 0; j < events[i].Count; j++)
                {
                    // Reset state to correct handle time events on playables.
                    if (events[i].phases[j] == Phase.LoopStarted)
                    {
                        _currentTime = startTime;

                        for (int k = 0; k < _playables.Count; k++)
                            _playables[k].playable._currentTime = playablesStartTime;

                        // When duration == 0, we jusst need call event.
                        if (Mathf.Approximately(FullDuration, 0f))
                        {
                            SetCurrentTime(events[i].time, events[i].phases[j]);
                            events[i].Call(j);

                            continue;
                        }
                    }

                    currentTime = LoopTime(WrapTime(_currentTime, loopDuration, events[i].phases[j]), _loopType) * _duration;
                    nextTime = WholeLoopTime(WrapTime(events[i].time, loopDuration, events[i].phases[j])) * _duration;

                    SetTimeOnPlayables(currentTime, nextTime);
                    SetCurrentTime(events[i].time, events[i].phases[j]);

                    events[i].Call(j);
                }
            }

            CheckAndSetMirrorHalfTime(loopDuration, subloopDuration, events[events.Count - 1].time);

            // Calling update or complete and loop complete events.
            currentTime = WholeLoopTime(WrapTime(_currentTime, loopDuration, _currentTimePhase)) * _duration;
            nextTime = LoopTime(WrapTime(events[events.Count - 1].time, loopDuration, events[events.Count - 1].phases[0] == Phase.LoopUpdated ? Phase.LoopUpdated : Phase.Completed), _loopType) * _duration;

            SetTimeOnPlayables(currentTime, nextTime);
            SetCurrentTime(events[events.Count - 1].time, events[events.Count - 1].phases[events[events.Count - 1].phases.Count - 1]);

            events[events.Count - 1].CallAll();
        }

        protected float WholeLoopTime(float time)
        {
            if (_loopType == LoopType.Mirror)
                return LoopTime(time, _loopType);

            return LoopTime(time, Mathf.Approximately(GetLoopedStartTime(), -1f) ? LoopType.Backward : LoopType.Forward);
        }

        protected float GetLoopedStartTime()
        {
            var direction = _loopType == LoopType.Backward ? -1f : 1f;
            direction *= _parent?.GetLoopedStartTime() ?? 1f;

            return direction;
        }

        // Make child playables play to end when mirror loop type used.
        protected void CheckAndSetMirrorHalfTime(float loopDuration, float subloopDuration, float nextTime)
        {
            if (_loopType != LoopType.Mirror)
                return;

            // When duration is 0 we need special handle mirror half time.
            if (Mathf.Approximately(loopDuration, 0f))
            {
                SetTimeOnPlayables(_currentTime, nextTime);
                return;
            }

            var period = Mathf.FloorToInt(nextTime / subloopDuration);

            if (period % 2 == 0)
                return;

            var middle = period * subloopDuration;

            if ((_currentTime < nextTime && (middle <= _currentTime || middle > nextTime)) || (_currentTime > nextTime && (middle <= nextTime || middle > _currentTime)))
                return;

            var currentTime = LoopTime(WrapTime(_currentTime, loopDuration, _currentTimePhase), _loopType) * _duration;
            nextTime = LoopTime(WrapTime(middle, loopDuration, Phase.LoopUpdated), _loopType) * _duration;

            SetTimeOnPlayables(currentTime, nextTime);
            SetCurrentTime(middle, Phase.LoopUpdated);
        }

        protected void SetTimeOnPlayables(float currentTime, float time)
        {
            var playables = GetCurrentPlayables(currentTime, time);

            for (int k = 0; k < playables.Count; k++)
                playables[k].playable.SetTime(time - playables[k].time);
        }

        protected List<(int order, float time, Playable playable)> GetCurrentPlayables(float currentTime, float time)
        {
            if (FullDuration == 0f)
            {
                var playables = new List<(int order, float time, Playable playable)>(_playables);

                if (currentTime < time)
                    playables.Sort((a, b) => a.order.CompareTo(b.order));
                else
                    playables.Sort((a, b) => a.order.CompareTo(b.order) * -1);

                return playables;
            }

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

            playables.Sort((a, b) => a.order.CompareTo(b.order) * -1);

            // Add last playables which can't be started later.
            if (time == 0f)
            {
                var lastPlayables = new List<(int order, float time, Playable playable)>();

                for (int i = 0; i < _playables.Count; i++)
                {
                    if (_playables[i].time + _playables[i].playable.FullDuration == 0f)
                        lastPlayables.Add(_playables[i]);
                }

                lastPlayables.Sort((a, b) => a.order.CompareTo(b.order) * -1);

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

        protected internal override void ResetCurrentTime(float time)
        {
            for (int i = 0; i < _playables.Count; i++)
                _playables[i].playable.ResetCurrentTime(time);

            _currentTime = time;
        }

        protected internal override void ResetState(float time)
        {

        }

        #region Event handlers
        protected void Playable_FullDurationChanged(Playable playable) => CalculateAllDurations();
        #endregion
    }
}