using UnityEngine;

namespace Game.Weather
{
    public class BirdObjectController : WeatherController
    {
        protected override float HeightOffset => 10f;
        protected override float CylinderHeight => 3f;
        protected override float SpawnCadence => 9f;
        protected override float Radius => 40f;
        protected override Vector2 LifetimeRange => new Vector2(20f, 30f);
        
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
