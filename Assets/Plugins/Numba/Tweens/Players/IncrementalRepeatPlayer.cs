using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens;

namespace Numba.Tweens.Players
{
    public class IncrementalRepeatPlayer : Player
    {
        public IncrementalRepeatPlayer(Playable playable) : base(playable) { }

        protected override IEnumerator PlayEnumerator(int count)
        {
            while ((count = Mathf.Max(count - 1, -2)) != -1)
            {
                yield return _playable.Play();

                if (_playable.LoopType == LoopType.Mirror) continue;

                foreach (var tweaker in _playable.Tweakers)
                {
                    if (_playable.LoopType == LoopType.Forward)
                    {
                        var incrementedTo = tweaker.EvaluateObject(2f);
                        (tweaker.FromObject, tweaker.ToObject) = (tweaker.ToObject, incrementedTo);
                    }
                    else
                    {
                        var decrementedFrom = tweaker.EvaluateObject(-1f);
                        (tweaker.FromObject, tweaker.ToObject) = (decrementedFrom, tweaker.FromObject);
                    }
                }
            }

            _playEnumerator = null;
        }
    }

}