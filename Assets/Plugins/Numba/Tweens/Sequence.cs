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
        protected List<(int order, float time, Playable playable)> _playables = new List<(int order, float, Playable)>();

        protected int _nextOrder;

        public Sequence(int count = 1, LoopType loopType = LoopType.Forward) : this(null, count, loopType) { }

        public Sequence(string name, int count = 1, LoopType loopType = LoopType.Forward) : base(name, 0f, count, loopType) { }

        protected internal override List<Tweaker> Tweakers => throw new NotImplementedException();

        protected internal override void SetTime(float time, bool normalized = false)
        {
            if (!normalized)
                time = Mathf.Clamp01(time / FullDuration);

            var events = GetTimeShiftEvents(time);

            if (events == null)
                return;

            if (FullDuration == 0f)
            {
                
            }
            else if (events != null)
            {
                
            }
        }

        protected List<(int order, float time, Playable playable)> GetCurrentPlayables(float time)
        {
            var currentTime = _currentTime * FullDuration;
            var nextTime = time * FullDuration;

            var playables = new List<(int order, float time, Playable playable)>();

            for (int i = 0; i < _playables.Count; i++)
            {
                if (_playables[i].time + _playables[i].playable.FullDuration >= currentTime && _playables[i].time < nextTime)
                    playables.Add(_playables[i]);
            }

            playables.Sort((a, b) => a.order.CompareTo(b.order));

            // Add last playables which can't be started later.
            if (nextTime == 1f)
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