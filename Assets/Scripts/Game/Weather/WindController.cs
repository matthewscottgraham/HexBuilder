using System.Collections.Generic;
using App.Tweens;
using UnityEngine;

namespace Game.Weather
{
    public class WindController : WeatherController
    {
        protected override float HeightOffset => 6f;
        protected override float CylinderHeight => 6f;
        protected override float SpawnCadence => 9f;
        protected override float Radius => 30f;
        protected override Vector2 LifetimeRange => new Vector2(20f, 30f);

        protected override void OnGetObject(GameObject obj)
        {
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            obj.transform.localScale = Vector3.one * Random.Range(0.8f, 2f);
            var tween = meshRenderer.material.TweenAlpha(0, 1, 3f);
            SetObjectAnimation(obj);
        }
        
        protected override void SetObjectAnimation(GameObject obj)
        {
            var position = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var lifeTime = Random.Range(LifetimeRange.x, LifetimeRange.y);
            obj.transform.position = position;
            
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            var tween = meshRenderer.material.TweenAlpha(1, 0, 3f).SetDelay(lifeTime - 3)
                .SetOnComplete(() => HandleTweenComplete(obj));
            _tweens[obj] = tween;
        }
    }
}
