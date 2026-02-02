using System;
using System.Collections.Generic;
using App.Events;
using App.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace App.VFX
{
    public class VFXController : MonoBehaviour, IDisposable
    {
        protected const int MaxPoolSize = 35;

        private EventBinding<PlayVFXBurstEvent> _playVFXEventBinding;
        private EventBinding<GamePauseEvent> _gamePauseEventBinding;
        private EventBinding<GameResumeEvent> _gameResumeEventBinding;
        
        protected ObjectPool<GameObject> VisualEffectPool;
        protected readonly HashSet<GameObject> ActiveVisualEffects = new();
        
        
        public void Initialize()
        {
            VisualEffectPool = new ObjectPool<GameObject>(CreateVisualEffect, GetVisualEffect, ReleaseVisualEffect);
            
            _playVFXEventBinding = new EventBinding<PlayVFXBurstEvent>(HandlePlayVFXBurstEvent);
            EventBus<PlayVFXBurstEvent>.Register(_playVFXEventBinding);
            
            _gamePauseEventBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            EventBus<GamePauseEvent>.Register(_gamePauseEventBinding);
            
            _gameResumeEventBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            EventBus<GameResumeEvent>.Register(_gameResumeEventBinding);
        }
        public virtual void Dispose()
        {
            EventBus<PlayVFXBurstEvent>.Deregister(_playVFXEventBinding);
            EventBus<GamePauseEvent>.Deregister(_gamePauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_gameResumeEventBinding);
            _playVFXEventBinding = null;
            _gamePauseEventBinding = null;
            _gameResumeEventBinding = null;
            VisualEffectPool.Clear();
            ServiceLocator.Instance.Deregister(this);
        }
        public virtual void RegisterVFX(string vfxID, object prefab)
        {
            // NOOP
        }

        protected virtual void HandlePlayVFXBurstEvent(PlayVFXBurstEvent evt)
        {
            // NOOP
        }

        protected virtual void SetPauseStateOnActiveVFX(bool isPaused)
        {
            // NOOP
        }

        protected virtual GameObject CreateVisualEffect()
        {
            // NOOP
            return null;
        }

        protected virtual void ReleaseVisualEffect(GameObject vfxObject)
        {
            // NOOP
        }

        private void HandleGamePause()
        {
            SetPauseStateOnActiveVFX(true);
        }

        private void HandleGameResume()
        {
            SetPauseStateOnActiveVFX(false);
        }

        private void GetVisualEffect(GameObject vfxObject)
        {
            ActiveVisualEffects.Add(vfxObject);
        }
    }
}