using System;
using System.Collections;
using System.Collections.Generic;
using App.Events;
using App.Services;
using App.Utils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace App.VFX
{
    public class VFXController : MonoBehaviour, IDisposable
    {
        private const int MaxPoolSize = 35;

        private EventBinding<PlayVFXBurstEvent> _playVFXEventBinding;
        private EventBinding<GamePauseEvent> _gamePauseEventBinding;
        private EventBinding<GameResumeEvent> _gameResumeEventBinding;
        
        private ObjectPool<VisualEffect> _visualEffectPool;
        private readonly HashSet<VisualEffect> _activeVisualEffects = new();
        private Dictionary<string, VisualEffectAsset> _visualEffectAssets = new();
        
        public void Initialize()
        {
            _visualEffectPool = new ObjectPool<VisualEffect>(CreateVisualEffect, GetVisualEffect, ReleaseVisualEffect);
            
            _playVFXEventBinding = new EventBinding<PlayVFXBurstEvent>(HandlePlayVFXBurstEvent);
            EventBus<PlayVFXBurstEvent>.Register(_playVFXEventBinding);
            
            _gamePauseEventBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            EventBus<GamePauseEvent>.Register(_gamePauseEventBinding);
            
            _gameResumeEventBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            EventBus<GameResumeEvent>.Register(_gameResumeEventBinding);
        }

        public void Dispose()
        {
            EventBus<PlayVFXBurstEvent>.Deregister(_playVFXEventBinding);
            EventBus<GamePauseEvent>.Deregister(_gamePauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_gameResumeEventBinding);
            _playVFXEventBinding = null;
            _gamePauseEventBinding = null;
            _gameResumeEventBinding = null;
            _visualEffectPool.Clear();
            _visualEffectAssets.Clear();
            ServiceLocator.Instance.Deregister(this);
        }
        
        public void RegisterVFX(string vfxID, VisualEffectAsset vfxAsset)
        {
            if (_visualEffectAssets.ContainsKey(vfxID)) return;
            _visualEffectAssets.Add(vfxID, vfxAsset);
        }

        private void HandlePlayVFXBurstEvent(PlayVFXBurstEvent evt)
        {
            if (_visualEffectPool.CountInactive == 0 && _visualEffectPool.CountAll >= MaxPoolSize) return;
            var vfx = _visualEffectPool.Get();
            vfx.visualEffectAsset = _visualEffectAssets[evt.EffectID];
            vfx.transform.position = evt.Position;
            vfx.transform.rotation = Quaternion.Euler(evt.Rotation);
            vfx.Play();
            StartCoroutine(ReleaseVFXWhenFinished(vfx));
        }

        private IEnumerator ReleaseVFXWhenFinished(VisualEffect vfx)
        {
            yield return new WaitUntil(() => vfx.aliveParticleCount == 0);
            _visualEffectPool.Release(vfx);
        }

        private void HandleGamePause()
        {
            SetPauseStateOnActiveVFX(true);
        }

        private void HandleGameResume()
        {
            SetPauseStateOnActiveVFX(false);
        }

        private void SetPauseStateOnActiveVFX(bool isPaused)
        {
            foreach (var vfx in _activeVisualEffects)
            {
                vfx.pause = isPaused;
            }
        }

        private VisualEffect CreateVisualEffect()
        {
            return gameObject.AddChild<VisualEffect>("VFX");
        }

        private void GetVisualEffect(VisualEffect vfx)
        {
            _activeVisualEffects.Add(vfx);
        }

        private void ReleaseVisualEffect(VisualEffect vfx)
        {
            vfx.visualEffectAsset = null;
            if (_activeVisualEffects.Contains(vfx))
                _activeVisualEffects.Remove(vfx);
        }
    }
}
