using System;
using System.Collections;
using System.Collections.Generic;
using App.Events;
using App.Utils;
using UnityEngine;
using UnityEngine.VFX;

namespace App.VFX
{
    public class GpuVfxController : VFXController, IDisposable
    {
        private readonly Dictionary<string, VisualEffectAsset> _visualEffectAssets = new();

        public override void Dispose()
        {
            _visualEffectAssets.Clear();
            base.Dispose();
        }
        public override void RegisterVFX(string vfxID, object prefab)
        {
            var vfxAsset = (VisualEffectAsset)prefab;
            if (vfxAsset == null) return;
            _visualEffectAssets.TryAdd(vfxID, vfxAsset);
        }
        
        protected override void HandlePlayVFXBurstEvent(PlayVFXBurstEvent evt)
        {
            if (VisualEffectPool.CountInactive == 0 && VisualEffectPool.CountAll >= MaxPoolSize) return;
            var vfxObject = VisualEffectPool.Get();
            var vfx = vfxObject.GetComponent<VisualEffect>();
            vfx.visualEffectAsset = _visualEffectAssets[evt.EffectID];
            vfx.transform.position = evt.Position;
            vfx.transform.rotation = Quaternion.Euler(evt.Rotation);
            vfx.Play();
            StartCoroutine(ReleaseVFXWhenFinished(vfx));
        }

        protected override void SetPauseStateOnActiveVFX(bool isPaused)
        {
            foreach (var vfxObject in ActiveVisualEffects)
            {
                vfxObject.GetComponent<VisualEffect>().enabled = isPaused;
            }
        }

        protected override GameObject CreateVisualEffect()
        {
            var visualEffect = gameObject.AddChild<VisualEffect>("VFX");
            return visualEffect.gameObject;
        }

        protected override void ReleaseVisualEffect(GameObject vfxObject)
        {
            var visualEffect = vfxObject.GetComponent<VisualEffect>();
            visualEffect.visualEffectAsset = null;
            if (ActiveVisualEffects.Contains(vfxObject))
                ActiveVisualEffects.Remove(vfxObject);
        }

        private IEnumerator ReleaseVFXWhenFinished(VisualEffect vfx)
        {
            yield return new WaitUntil(() => vfx.aliveParticleCount == 0);
            VisualEffectPool.Release(vfx.gameObject);
        }
    }
}
