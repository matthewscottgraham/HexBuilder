using App.Tweens;
using UnityEngine;

namespace Game.Weather
{
    public class CloudObjectController : WeatherController
    {
        protected override float HeightOffset => 15f;
        protected override float CylinderHeight => 1f;
        protected override float SpawnCadence => 9f;
        protected override float Radius => 40f;
        protected override Vector2 LifetimeRange => new Vector2(20f, 30f);

        protected override void OnGetObject(GameObject obj)
        {
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            var tween = meshRenderer.material.TweenAlpha(0, 1, 3f);
            SetObjectAnimation(obj);
        }
        
        protected override void SetObjectAnimation(GameObject obj)
        {
            var startPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var endPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var lifeTime = Random.Range(LifetimeRange.x, LifetimeRange.y);
            obj.transform.position = startPos;
            
            var tween = obj.transform.TweenPosition(startPos, endPos, lifeTime)
                .SetOnComplete(() => HandleTweenComplete(obj));
            _tweens[obj] = tween;
            
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            var tween2 = meshRenderer.material.TweenAlpha(1, 0, 3f).SetDelay(lifeTime - 3);
        }
        
        protected override Vector3 GetRandomPointInCylinder(float radius, float cylinderHeight, float heightOffset)
        {
            var angle = Random.value * Mathf.PI * 2;
        
            return new Vector3(
                Mathf.Cos(angle) * radius,
                Random.Range(heightOffset, heightOffset + cylinderHeight),
                Mathf.Sin(angle) * radius
            );
        }
    }
}