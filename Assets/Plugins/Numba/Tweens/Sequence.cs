using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Exceptions;

namespace Numba.Tweens
{
    public sealed class Sequence : Playable
    {
        private Phase _currentTimePhase;

        private List<(int order, float time, Playable playable)> _playables = new List<(int order, float, Playable)>();

        private int _nextOrder;

        private RewindType _rewindType;

        public RewindType RewindType
        {
            get => _rewindType;
            set
            {
                if (IsBusy) ThrowChangeBusyException("rewind type");

                _rewindType = Enum.IsDefined(typeof(RewindType), value) ? value : throw new ArgumentException("Rewind type must be rewind or untouched");
            }
        }

        public Sequence(int count = 1, LoopType loopType = LoopType.Forward, RewindType rewindType = RewindType.Rewind) : this(null, count, loopType, rewindType) { }

        public Sequence(string name, int count = 1, LoopType loopType = LoopType.Forward, RewindType rewindType = RewindType.Rewind) : base(name, 0f, count, loopType)
        {
            _rewindType = rewindType;
        }

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

            // In mirror mode it is no matter from what end we start.
            float childsPlayingStartTime = 0f;

            if (_loopType != LoopType.Mirror)
            {
                // If we move forward, then current time for childs must be 0 when started,
                // otherwise (when go in backward direction) it must be 1.
                childsPlayingStartTime = _currentTime < time ? 0f : 1f;

                // When we use backward loop type, start time for childs must be reversed (even in difficult hierarchy).
                if (_loopType == LoopType.Backward)
                    childsPlayingStartTime = 1f - childsPlayingStartTime;
            }

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
                        if (_loopType != LoopType.Mirror)
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
                        if (currentTime > time)
                            (currentTime, nextTime) = (nextTime, currentTime);

                        SetTimeOnPlayables(currentTime, nextTime, true, _currentTime < events[i].time ? 1 : -1);
                        (currentTime, nextTime) = (nextTime, currentTime);
                    }

                    SetTimeOnPlayables(currentTime, nextTime, true, _currentTime < events[i].time ? 1 : -1);

                    events[i].Call(j);
                }
            }

            currentTime = _currentTime;
            nextTime = events[events.Count - 1].time;

            if (_loopType == LoopType.Backward)
                (currentTime, nextTime) = (nextTime, currentTime);
            else if (_loopType == LoopType.Mirror)
            {
                if (currentTime > time)
                    (currentTime, nextTime) = (nextTime, currentTime);

                SetTimeOnPlayables(currentTime, nextTime, true, _currentTime < events[events.Count - 1].time ? 1 : -1);
                (currentTime, nextTime) = (nextTime, currentTime);
            }

            SetTimeOnPlayables(currentTime, nextTime, true, _currentTime < events[events.Count - 1].time ? 1 : -1);
            SetCurrentTime(events[events.Count - 1].time, events[events.Count - 1].phases[events[events.Count - 1].phases.Count - 1]);

            events[events.Count - 1].CallAll();
        }

        private void SetTimeWhenDurationIsNotZero(float time, bool normalized = false)
        {
            if (!GetEvents(ref time, normalized, out Events events))
                return;

            int startIndex = 0;

            // Self playing start time.
            var playingStartTime = _currentTime < time ? 0f : 1f;

            // In mirror mode it is no matter from what end we start childs.
            float childsPlayingStartTime = 0f;

            if (_loopType != LoopType.Mirror)
            {
                // If we move forward, then current time for childs must be 0 when started,
                // otherwise (when go in backward direction) it must be 1.
                childsPlayingStartTime = _currentTime < time ? 0f : 1f;

                // When we use backward loop type, start time for childs must be reversed (even in difficult hierarchy).
                if (_loopType == LoopType.Backward)
                    childsPlayingStartTime = 1f - childsPlayingStartTime;
            }

            // Calling start and loop start events.
            if (events[0].phases[0] == Phase.Started)
            {
                _currentTimePhase = Phase.LoopStarted;

                SetPlayablesStartTime(childsPlayingStartTime);

                if (_rewindType == RewindType.Rewind)
                {
                    for (int k = 0; k < _playables.Count; k++)
                        _playables[k].playable.ResetStateAccordingToTime(childsPlayingStartTime);
                }

                // We need just call events, because no one playable can't be started in this time.
                events[0].CallAll();
                startIndex = 1;
            }

            var normalizedDuration = GetNormalizedDuration();
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
                        SetCurrentTime(events[i].time, Phase.LoopStarted);

                        if (_loopType != LoopType.Mirror)
                        {
                            SetPlayablesStartTime(childsPlayingStartTime);

                            if (_rewindType == RewindType.Rewind)
                            {
                                for (int k = 0; k < _playables.Count; k++)
                                    _playables[k].playable.ResetStateAccordingToTime(childsPlayingStartTime);
                            }
                        }

                        events[i].Call(j);

                        // It is no reason to handle playables because no one playable will played when
                        // current time and next time have the same value.
                        continue;
                    }

                    if (_loopType == LoopType.Mirror)
                        HandleMirrorMode(normalizedDuration, events[i].time, playingStartTime);

                    currentTime = _currentTimePhase == Phase.LoopStarted ? playingStartTime : WrapTime(_currentTime, normalizedDuration);
                    currentTime = LoopTime(currentTime, _loopType) * _duration;

                    nextTime = LoopTime(WrapTime(events[i].time, normalizedDuration) - playingStartTime, _loopType) * _duration;

                    SetTimeOnPlayables(currentTime, nextTime, false, _currentTime < events[i].time ? 1 : -1);
                    SetCurrentTime(events[i].time, events[i].phases[j]);

                    events[i].Call(j);
                }
            }

            if (_loopType == LoopType.Mirror)
                HandleMirrorMode(normalizedDuration, events[events.Count - 1].time, playingStartTime);

            currentTime = _currentTimePhase == Phase.LoopStarted ? playingStartTime : WrapTime(_currentTime, normalizedDuration);
            currentTime = LoopTime(currentTime, _loopType) * _duration;

            nextTime = WrapTime(events[events.Count - 1].time, normalizedDuration);

            // when we stopped on loop completed event state it is need substract playing start time again,
            // otherwise tweaker will take wrong parameter value.
            if (events[events.Count - 1].Count == 1 && events[events.Count - 1].phases[0] == Phase.LoopCompleted)
                nextTime -= playingStartTime;

            nextTime = LoopTime(nextTime, _loopType) * _duration;

            SetTimeOnPlayables(currentTime, nextTime, false, _currentTime < events[events.Count - 1].time ? 1 : -1);
            SetCurrentTime(events[events.Count - 1].time, events[events.Count - 1].phases[events[events.Count - 1].Count - 1]);

            events[events.Count - 1].CallAll();
        }

        private void HandleMirrorMode(float normalizedDuration, float eventTime, float playingStartTime)
        {
            var normalizedMirrorTime = normalizedDuration / 2f;
            var subPeriod = eventTime / normalizedMirrorTime;

            float middle;

            if (Mathf.FloorToInt(subPeriod) % 2 == 0)
            {
                var period = eventTime / normalizedDuration;
                var flooredPeriod = Mathf.FloorToInt(period);

                if (Mathf.Approximately(period, flooredPeriod))
                    middle = (_currentTime < eventTime ? subPeriod - 1f : subPeriod + 1f) * normalizedMirrorTime;
                else
                    middle = Mathf.Ceil(subPeriod) * normalizedMirrorTime;
            }
            else
                middle = Mathf.Floor(subPeriod) * normalizedMirrorTime;

            if ((_currentTime < eventTime && (middle <= _currentTime || middle >= eventTime)) || (_currentTime > eventTime && (middle >= _currentTime || middle <= eventTime)))
                return;

            float currentTime = _currentTimePhase == Phase.LoopStarted ? playingStartTime : WrapTime(_currentTime, normalizedDuration);
            currentTime = LoopTime(currentTime, _loopType) * _duration;

            var nextTime = LoopTime(WrapTime(middle, normalizedDuration), _loopType) * _duration;

            SetTimeOnPlayables(currentTime, nextTime, false, _currentTime < middle ? 1 : -1);
            SetCurrentTime(middle, Phase.LoopUpdated);
        }

        private void SetPlayablesStartTime(float time)
        {
            for (int i = 0; i < _playables.Count; i++)
                _playables[i].playable._currentTime = time;
        }

        private void SetTimeOnPlayables(float startTime, float endTime, bool isZeroDuration, int verticalSorting)
        {
            var playables = GetPlayables(startTime, endTime, isZeroDuration, verticalSorting);

            // This required for situations when zero duration playables placed at end (in both directions) of sequence,
            // otherwise the SetTime method on playables will be not working.
            if (startTime < endTime && Mathf.Approximately(endTime, FullDuration))
                endTime += 1f;
            else if (startTime > endTime && Mathf.Approximately(endTime, 0f))
                endTime -= 1f;

            for (int k = 0; k < playables.Count; k++)
                playables[k].playable.SetTime(endTime - playables[k].time);
        }

        private List<(int order, float time, Playable playable)> GetPlayables(float startTime, float endTime, bool isZeroDuration, int verticalSorting)
        {
            if (isZeroDuration)
                return startTime < endTime ? GetForwardPlayables(0f, 0f, verticalSorting) : GetBackwardPlayables(0f, 0f, verticalSorting);
            else
                return startTime < endTime ? GetForwardPlayables(startTime, endTime, verticalSorting) : GetBackwardPlayables(startTime, endTime, verticalSorting);
        }

        private List<(int order, float time, Playable playable)> GetForwardPlayables(float startTime, float endTime, int verticalSorting)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time + _playables[i].playable.FullDuration >= startTime && _playables[i].time < endTime)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order) * verticalSorting);

            if (endTime == FullDuration)
            {
                var lastPlayables = new List<(int order, float time, Playable playable)>();

                for (int i = 0; i < _playables.Count; i++)
                {
                    if (_playables[i].time == FullDuration)
                        lastPlayables.Add(_playables[i]);
                }

                lastPlayables.Sort((a, b) => a.order.CompareTo(b.order) * verticalSorting);

                playables.AddRange(lastPlayables);
            }

            return playables;
        }

        private List<(int order, float time, Playable playable)> GetBackwardPlayables(float startTime, float endTime, int verticalSorting)
        {
            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time <= startTime && _playables[i].time + _playables[i].playable.FullDuration > endTime)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order) * verticalSorting);

            // Add last playables which can't be started later.
            if (endTime == 0f)
            {
                var lastPlayables = new List<(int order, float time, Playable playable)>();

                for (int i = 0; i < _playables.Count; i++)
                {
                    if (_playables[i].time + _playables[i].playable.FullDuration == 0f)
                        lastPlayables.Add(_playables[i]);
                }

                lastPlayables.Sort((a, b) => a.order.CompareTo(b.order) * verticalSorting);

                playables.AddRange(lastPlayables);
            }

            return playables;
        }

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
            if (playable == null)
                throw new ArgumentNullException(nameof(playable));

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

        protected internal override void ResetCurrentTime()
        {
            for (int i = 0; i < _playables.Count; i++)
                _playables[i].playable.ResetCurrentTime();

            _currentTime = 0f;
        }

        protected internal override void ResetStateAccordingToTime(float time)
        {
            if (_loopType == LoopType.Mirror)
                time = 0f;
            else if (_loopType == LoopType.Backward)
                time = 1f - time;

            for (int i = 0; i < _playables.Count; i++)
                _playables[i].playable.ResetStateAccordingToTime(time);
        }

        protected override void CheckSpecificPlayExceptions()
        {
            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].playable.IsBusy)
                    throw new BusyException($"Child playable with name \"{_playables[i].playable.Name}\" already playing");
            }
        }

        #region Event handlers
        private void Playable_FullDurationChanged(Playable playable) => CalculateAllDurations();
        #endregion
    }
}