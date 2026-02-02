using System.Collections;
using System.Collections.Generic;
using App.Events;
using App.Utils;
using UnityEngine;

namespace App.VFX
{
    public class CpuVfxController : VFXController
    {
        private readonly Dictionary<string, ParticleSystem> _visualEffectAssets = new();

        public override void Dispose()
        {
            _visualEffectAssets.Clear();
            base.Dispose();
        }
        public override void RegisterVFX(string vfxID)
        {
            var prefab = Resources.Load<ParticleSystem>($"VFX/{vfxID}");
            if (!prefab) return;
            _visualEffectAssets.TryAdd(vfxID, prefab);
        }

        public override GameObject GetPersistentVFX(string vfxID)
        {
            if (!_visualEffectAssets.TryGetValue(vfxID, out var vfxPrefab)) return null;
            var vfxObject = CreateVisualEffect();
            var vfx = vfxObject.GetComponent<ParticleSystem>();
            ParticleSystemCopier.CopyParticleSystem(vfxPrefab, vfx);
            vfx.Play();
            return vfxObject;
        }
        
        protected override void HandlePlayVFXBurstEvent(PlayVFXBurstEvent evt)
        {
            if (VisualEffectPool.CountInactive == 0 && VisualEffectPool.CountAll >= MaxPoolSize) return;
            var vfxObject = VisualEffectPool.Get();
            var vfx = vfxObject.GetComponent<ParticleSystem>();
            ParticleSystemCopier.CopyParticleSystem(_visualEffectAssets[evt.EffectID], vfx);
            vfx.transform.position = evt.Position;
            vfx.transform.rotation = Quaternion.Euler(evt.Rotation);
            vfx.Play();
            StartCoroutine(ReleaseVFXWhenFinished(vfx));
        }

        protected override void SetPauseStateOnActiveVFX(bool isPaused)
        {
            foreach (var vfxObject in ActiveVisualEffects)
            {
                var ps = vfxObject.GetComponent<ParticleSystem>();
                if (isPaused) ps.Pause();
                else ps.Play();
            }
        }

        protected override GameObject CreateVisualEffect()
        {
            var visualEffect = gameObject.AddChild<ParticleSystem>("VFX");
            return visualEffect.gameObject;
        }

        protected override void ReleaseVisualEffect(GameObject vfxObject)
        {
            var ps = vfxObject.GetComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (ActiveVisualEffects.Contains(vfxObject))
                ActiveVisualEffects.Remove(vfxObject);
        }

        private IEnumerator ReleaseVFXWhenFinished(ParticleSystem vfx)
        {
            yield return new WaitUntil(() => !vfx.IsAlive(true));
            VisualEffectPool.Release(vfx.gameObject);
        }
    }
}