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
        protected List<(float time, Playable playable)> _playables = new List<(float, Playable)>();

        public Sequence(int count = 1, LoopType loopType = LoopType.Forward) : this(null, count, loopType) { }

        public Sequence(string name, int count = 1, LoopType loopType = LoopType.Forward) : base(name, 0f, count, loopType) { }

        protected internal override List<Tweaker> Tweakers => throw new NotImplementedException();

        protected internal override void SetTime(float time, bool normalized = false)
        {
            var events = GetTimeShiftEvents(ref time, normalized);

            if (FullDuration == 0f)
            {
                // Invoke start and loop start events.
                events[0].Call();

                // Invoke all event sequentially in all playables.
                for (int i = 0; i < _playables.Count; i++)
                    _playables[i].playable.SetTime(_playables[i].playable.FullDuration);

                // Invoke complete and loop complete events.
                events[1].Call();
            }

            if (events != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    _currentTime = events[i].time;

                    events[i].Call();
                }
            }
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

        public void Append(Playable playable) => Append(playable, _playables.Count);

        public void Append(Playable playable, int order) => Insert(_duration, playable, order);

        public void Insert(float time, Playable playable) => Insert(time, playable, _playables.Count);

        public void Insert(float time, Playable playable, int order)
        {
            if (playable == this)
                throw new ArgumentException($"Sequence \"{Name}\" can't contain itself");

            if (playable is Sequence sequence && CheckOnCyclicReference(sequence))
                throw new ArgumentException($"Cyclic references detected. Sequence \"{playable.Name}\" or its child sequences (in whole hierarchy) already referenced to sequence \"{Name}\"");

            if (_playables.Contains((order, playable)))
                throw new ArgumentException($"Sequence \"{Name}\" already contains playable with name \"{playable.Name}\"");

            _playables.Insert(order, (Mathf.Max(time, 0f), playable));
            playable._parent = this;

            playable.FullDurationChanged += Playable_FullDurationChanged;

            CalculateAllDurations();
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