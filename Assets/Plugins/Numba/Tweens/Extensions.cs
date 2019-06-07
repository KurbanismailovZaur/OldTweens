using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Players;

namespace Numba.Tweens
{
    public static class Extensions
    {
        public static Player PlayRepeated(this Playable playable, int count = -1) => new RepeatPlayer(playable).Play(count);

		public static Player PlayIncrementalRepeated(this Playable playable, int count = -1) => new IncrementalRepeatPlayer(playable).Play(count);
    }
}