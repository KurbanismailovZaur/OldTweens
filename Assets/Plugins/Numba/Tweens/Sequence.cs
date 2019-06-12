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

        protected override void SetTime(float time, bool normalized = false)
        {
            var events = GetTimeShiftEvents(time, normalized);

            if (events != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].Call();
                }
            }
        }

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

        public void Append(Playable playable, int order)
        {
            if (playable == this)
                throw new ArgumentException($"Sequence \"{Name}\" can't contain itself");

            if (playable is Sequence sequence && CheckOnCyclicReference(sequence))
                throw new ArgumentException($"Cyclic references detected. Sequence \"{playable.Name}\" or its child sequences (in whole hierarchy) already referenced to sequence \"{Name}\"");

            if (_playables.Contains((order, playable)))
                throw new ArgumentException($"Sequence \"{Name}\" already contains playable with name \"{playable.Name}\"");

			_playables.Insert(order, (_duration, playable));
			
			_duration += playable.FullDuration;
			CalculateFullDuration();
        }
    }
}