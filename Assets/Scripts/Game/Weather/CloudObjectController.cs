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
        protected override Vector2 LifetimeRange => new (30f, 40f);

        protected override void OnGetObject(GameObject obj)
        {
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            obj.transform.localScale = Vector3.one * Random.Range(0.5f, 1f);
            meshRenderer.material.TweenAlpha(0, 1, 3f);
            obj.SetActive(true);
            SetObjectAnimation(obj);
        }
        
        protected override void SetObjectAnimation(GameObject obj)
        {
            var startPos = GetRandomPointOnEdge(Radius, CylinderHeight, HeightOffset, -1);
            var endPos = GetRandomPointOnEdge(Radius, CylinderHeight, HeightOffset, 1);
            var lifeTime = Random.Range(LifetimeRange.x, LifetimeRange.y);
            obj.transform.position = startPos;
            
            var tween = obj.transform.TweenPosition(startPos, endPos, lifeTime)
                .SetOnComplete(() => HandleTweenComplete(obj));
            Tweens[obj] = tween;
            
            var meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
            meshRenderer.material.TweenAlpha(1, 0, 3f).SetDelay(lifeTime - 3);
        }


        private static Vector3 GetRandomPointOnEdge(float radius, float height, float offset, int direction)
        {
            return new Vector3(
                radius * direction,
                Random.Range(offset, offset + height),
                Random.Range(-radius, radius)
            );
        }
    }
}