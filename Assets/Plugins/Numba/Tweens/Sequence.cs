using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Numba.Tweens
{
    public sealed class Sequence : Playable
    {
        private Phase _currentTimePhase;

        private List<(int order, float time, Playable playable)> _playables = new List<(int order, float, Playable)>();

        private int _nextOrder;

        public Sequence(int count = 1, LoopType loopType = LoopType.Forward) : this(null, count, loopType) { }

        public Sequence(string name, int count = 1, LoopType loopType = LoopType.Forward) : base(name, 0f, count, loopType) { }

        private void SetCurrentTime(float time, Phase phase)
        {
            _currentTime = time;
            _currentTimePhase = phase;
        }

        internal override void SetTime(float time, bool normalized = false)
        {
            if (Mathf.Approximately(FullDuration, 0f))
                SetTimeWhenDurationIsZero(time, normalized);
            else
                SetTimeWhenDurationIsNotZero(time, normalized);
        }

        private void SetTimeWhenDurationIsZero(float time, bool normalized = false)
        {
            if (!GetEvents(ref time, normalized, out Events events))
                return;

            int startIndex = 0;

            var childsPlayingStartTime = Mathf.Approximately(GetHierarchyPlayingDirection() * GetSelfPlayingDirection(), 1f) ? 0f : 1f;

            // It is need to reverse playing time for childs when sequence go in backward direction.
            if (_currentTime > time && _loopType != LoopType.Mirror)
                childsPlayingStartTime = 1f - childsPlayingStartTime;

            // Calling start and loop start events.
            if (events[0].phases[0] == Phase.Started)
            {
                _currentTimePhase = Phase.LoopStarted;

                SetPlayablesStartTime(childsPlayingStartTime);

                // We need just call events, because no one playable can't be started in this time.
                events[0].CallAll();
                startIndex = 1;
            }

            float currentTime;
            float nextTime;

            // Calling events between first (inclusive/exclusive) and last (exclusive).
            for (int i = startIndex; i < events.Count - 1; i++)
            {
                for (int j = 0; j < events[i].Count; j++)
                {
                    // It is need to reset current time phase when we achieve loop started phase,
                    // otherwise we catch all playables in backward direction later.
                    if (events[i].phases[j] == Phase.LoopStarted)
                    {
                        _currentTime = events[i].time;
                        _currentTimePhase = Phase.LoopStarted;

                        SetPlayablesStartTime(childsPlayingStartTime);

                        events[i].Call(j);

                        // It is no reason to handle playables because  no one playable will played when
                        // current time and next time have the same value.
                        continue;
                    }

                    currentTime = _currentTime;
                    nextTime = events[i].time;

                    if (_loopType == LoopType.Backward)
                        (currentTime, nextTime) = (nextTime, currentTime);
                    else if (_loopType == LoopType.Mirror)
                    {
                        if (_currentTime > time)
                            (currentTime, nextTime) = (nextTime, currentTime);

                        SetTimeOnPlayables(currentTime, nextTime, true);
                        (currentTime, nextTime) = (nextTime, currentTime);
                    }

                    SetTimeOnPlayables(currentTime, nextTime, true);
                    SetCurrentTime(events[i].time, events[i].phases[j]);

                    events[i].Call(j);
                }
            }

            currentTime = _currentTime;
            nextTime = events[events.Count - 1].time;

            if (_loopType == LoopType.Backward)
                (currentTime, nextTime) = (nextTime, currentTime);
            else if (_loopType == LoopType.Mirror)
            {
                if (_currentTime > time)
                    (currentTime, nextTime) = (nextTime, currentTime);

                SetTimeOnPlayables(currentTime, nextTime, true);
                (currentTime, nextTime) = (nextTime, currentTime);
            }

            SetTimeOnPlayables(currentTime, nextTime, true);
            SetCurrentTime(events[events.Count - 1].time, events[events.Count - 1].phases[events[events.Count - 1].phases.Count - 1]);

            events[events.Count - 1].CallAll();
        }

        private void SetTimeWhenDurationIsNotZero(float time, bool normalized = false)
        {

        }

        private void HandleMirrorMode(ref float currentTime, float normalizedDuration, float nextTime)
        {
            if (Mathf.Approximately(normalizedDuration, 0f))
            {
                SetTimeOnPlayables(currentTime, nextTime, true);

                // Here we can use 1f - GetPlayingStartTime() too, but it has less performance. 
                currentTime = 1f - currentTime;

                return;
            }

            var normalizedMirrorTime = normalizedDuration / 2f;
            var period = Mathf.FloorToInt(nextTime / normalizedMirrorTime);

            if (period % 2 == 0)
                return;

            var middle = period * normalizedMirrorTime;

            if ((_currentTime < nextTime && (middle <= _currentTime || middle >= nextTime)) || (_currentTime > nextTime && (middle >= _currentTime || middle <= nextTime)))
                return;

            currentTime *= _duration;
            var middletTime = LoopTime(WrapTime(middle, normalizedDuration, Phase.LoopUpdated), _loopType) * _duration;

            SetTimeOnPlayables(currentTime, middletTime, false);
            SetCurrentTime(middle, Phase.LoopUpdated);

            currentTime = LoopTime(WrapTime(_currentTime, normalizedDuration, _currentTimePhase), _loopType);
        }

        private void SetPlayablesStartTime(float time)
        {
            for (int i = 0; i < _playables.Count; i++)
                _playables[i].playable._currentTime = time;
        }

        private void SetTimeOnPlayables(float startTime, float endTime, bool isZeroDuration)
        {
            var playables = GetPlayables(startTime, endTime, isZeroDuration);

            for (int k = 0; k < playables.Count; k++)
                playables[k].playable.SetTime(endTime - playables[k].time);
        }

        private List<(int order, float time, Playable playable)> GetPlayables(float startTime, float endTime, bool isZeroDuration)
        {
            if (isZeroDuration)
                return startTime < endTime ? GetForwardPlayables(0f, 0f) : GetBackwardPlayables(0f, 0f);
            else
                return startTime < endTime ? GetForwardPlayables(startTime, endTime) : GetBackwardPlayables(startTime, endTime);
        }

        private List<(int order, float time, Playable playable)> GetForwardPlayables(float startTime, float endTime)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time + _playables[i].playable.FullDuration >= startTime && _playables[i].time < endTime)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order));

            if (endTime == FullDuration)
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

        private List<(int order, float time, Playable playable)> GetBackwardPlayables(float startTime, float endTime)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time <= startTime && _playables[i].time + _playables[i].playable.FullDuration > endTime)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order) * -1);

            // Add last playables which can't be started later.
            if (endTime == 0f)
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

        public void SetTimeIIIUUUHH(float time) => SetTime(time);

        private bool CheckOnCyclicReference(Sequence sequence)
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

        private void CalculateDuration()
        {
            _duration = 0f;

            for (int i = 0; i < _playables.Count; i++)
                _duration = Mathf.Max(_duration, _playables[i].time + _playables[i].playable.FullDuration);
        }

        private void CalculateAllDurations()
        {
            CalculateDuration();
            CalculateFullDuration();
        }

        #region Event handlers
        private void Playable_FullDurationChanged(Playable playable) => CalculateAllDurations();
        #endregion
    }
}