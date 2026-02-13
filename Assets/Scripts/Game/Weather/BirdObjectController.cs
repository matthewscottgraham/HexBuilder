using App.Tweens;
using UnityEngine;

namespace Game.Weather
{
    public class BirdObjectController : WeatherController
    {
        protected override float HeightOffset => 10f;
        protected override float CylinderHeight => 3f;
        protected override float SpawnCadence => 5f;
        protected override float Radius => 40f;
        protected override Vector2 LifetimeRange => new (20f, 30f);
        
        protected override Vector3 GetRandomPointInCylinder(float radius, float cylinderHeight, float heightOffset)
        {
            var angle = Random.value * Mathf.PI * 2;
        
            return new Vector3(
                Mathf.Cos(angle) * radius,
                Random.Range(heightOffset, heightOffset + cylinderHeight),
                Mathf.Sin(angle) * radius
            );
        }
        
        protected override void SetObjectAnimation(GameObject obj)
        {
            var startPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var endPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var lifeTime = Random.Range(LifetimeRange.x, LifetimeRange.y);
            obj.transform.position = startPos;
            obj.transform.LookAt(endPos);
            
            var tween = obj.transform.TweenPosition(startPos, endPos, lifeTime)
                .SetOnComplete(() => HandleTweenComplete(obj));
            
            Tweens[obj] = tween;
        }
    }
}
