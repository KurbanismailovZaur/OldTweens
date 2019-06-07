using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System;
using Numba.Tweens.Exceptions;

namespace Numba.Tweens
{
    public abstract class Player : CustomYieldInstruction
    {
        protected Playable _playable;

        public Player(Playable playable) => _playable = playable ?? throw new ArgumentNullException(nameof(playable));

		protected IEnumerator _playEnumerator;

        private Coroutine _playCoroutine;

		protected bool _needCompletePause;

        public PlayState PlayState => _playEnumerator == null ? PlayState.Stop : _playable.IsPaused ? PlayState.Pause : PlayState.Play;

        public bool IsPlaying => PlayState == PlayState.Play;

        public bool IsPaused => PlayState == PlayState.Pause;

        public bool IsStoped => PlayState == PlayState.Stop;

        public override bool keepWaiting => !IsStoped;

        public Player Play(int count = -1)
        {
            if (IsPlaying) throw new BusyException($"Player for playable with name \"{_playable.Name}\" already playing");

			if (IsStoped)
				_playEnumerator = PlayEnumerator(count);
			else
				_needCompletePause = true;

            _playCoroutine = CoroutineHelper.Instance.StartCoroutine(_playEnumerator);

            return this;
        }

        protected abstract IEnumerator PlayEnumerator(int count);

        public Player Pause()
        {
            if (!IsPlaying) return this;

			CoroutineHelper.Instance.StopCoroutine(_playCoroutine);

            _playable.Pause();

            return this;
        }

        public Player Stop()
        {
            if (IsStoped) return this;

            CoroutineHelper.Instance.StopCoroutine(_playCoroutine);
            _playEnumerator = null;

            _playable.Stop();

            return this;
        }
    }
}