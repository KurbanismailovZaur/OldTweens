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

        protected Coroutine _playCoroutine;

        protected Coroutine _continueCoroutine;

        public PlayState PlayState => _playEnumerator == null ? PlayState.Stop : _playable.IsPaused ? PlayState.Pause : PlayState.Play;

        public bool IsPlaying => PlayState == PlayState.Play;

        public bool IsPaused => PlayState == PlayState.Pause;

        public bool IsStoped => PlayState == PlayState.Stop;

        public override bool keepWaiting => !IsStoped;

        public Player Play(int count = -1)
        {
            if (IsPlaying) throw new BusyException($"Player for playable with name \"{_playable.Name}\" already playing");

            if (IsPaused)
                _continueCoroutine = CoroutineHelper.Instance.StartCoroutine(ContinueCoroutine(_playEnumerator));
            else
                _playCoroutine = CoroutineHelper.Instance.StartCoroutine(PlayEnumerator(count));

            return this;
        }

        protected abstract IEnumerator PlayEnumerator(int count);

        protected IEnumerator ContinueCoroutine(IEnumerator playEnumerator)
        {
            _playable.Play();
            yield return playEnumerator.Current;

            _playCoroutine = CoroutineHelper.Instance.StartCoroutine(playEnumerator);

            _continueCoroutine = null;
        }

        public Player Pause()
        {
            if (!IsPlaying) return this;

            if (_continueCoroutine != null)
            {
                CoroutineHelper.Instance.StopCoroutine(_continueCoroutine);
                _continueCoroutine = null;
            }
            else
                CoroutineHelper.Instance.StopCoroutine(_playCoroutine);

            _playable.Pause();

            return this;
        }

        public Player Stop()
        {
            if (IsStoped) return this;

            if (_continueCoroutine != null)
            {
                CoroutineHelper.Instance.StopCoroutine(_continueCoroutine);
                _continueCoroutine = null;
            }
            else if (_playCoroutine != null)
                CoroutineHelper.Instance.StopCoroutine(_playCoroutine);

            _playEnumerator = null;

            _playable.Stop();

            return this;
        }
    }
}