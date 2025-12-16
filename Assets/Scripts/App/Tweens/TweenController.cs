using System;
using System.Collections.Generic;
using System.Linq;
using App.Events;
using UnityEngine;

namespace App.Tweens
{
    public class TweenController : MonoBehaviour, IDisposable
    {
        private readonly Dictionary<string, ITween> _tweens = new();
        
        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;

        public void Dispose()
        {
            foreach (var tween in _tweens.Values.ToList()) tween.Kill();
            _tweens.Clear();
            EventBus<GamePauseEvent>.Deregister(_pauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeEventBinding);
        }

        public void Initialize()
        {
            // Todo create pool
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandleGamePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleGameResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
        }

        public void AddTween(ITween tween)
        {
            if (_tweens.ContainsKey(tween.ID))
            {
                _tweens[tween.ID].CompleteTween();
                RemoveTween(tween.ID);
            }

            _tweens.Add(tween.ID, tween);
        }

        private void HandleGamePauseEvent(GamePauseEvent gamePauseEvent)
        {
            foreach (var tween in _tweens.Values.ToList()) tween.Pause();
        }

        private void HandleGameResumeEvent(GameResumeEvent gameResumeEvent)
        {
            foreach (var tween in _tweens.Values.ToList()) tween.Resume();
        }

        private void Update()
        {
            foreach (var pair in _tweens.ToList())
            {
                var tween = pair.Value;
                tween.Tick();
                if (tween.IsComplete && !tween.WasKilled)
                {
                    if (tween.OnComplete != null)
                    {
                        tween.OnComplete.Invoke();
                        tween.OnComplete = null;
                    }

                    RemoveTween(tween.ID);
                }

                if (tween.WasKilled) RemoveTween(tween.ID);
            }
        }

        private void RemoveTween(string id)
        {
            if (!_tweens.ContainsKey(id)) return;
            _tweens.Remove(id);
            // TODO add back into pool
        }
    }
}