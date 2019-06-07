using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Exceptions;

namespace Numba.Tweens.Players
{
    public class RepeatPlayer : Player
    {
        public RepeatPlayer(Playable playable) : base(playable) { }
        protected override IEnumerator PlayEnumerator(int count)
        {
            while ((count = Mathf.Max(count - 1, -2)) != -1)
            {
                yield return _playable.Play();

                if (_needCompletePause)
                {
                    yield return _playable.Play();
                    _needCompletePause = false;
                }
            }

            _playEnumerator = null;
        }
    }
}