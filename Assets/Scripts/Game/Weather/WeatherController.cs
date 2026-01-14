using System.Collections.Generic;
using App.Tweens;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Weather
{
    public class WeatherController : MonoBehaviour
    {
        [SerializeField] protected GameObject[] prefabs;
        
        private ObjectPool<GameObject> _pool;
        protected readonly Dictionary<GameObject, ITween> _tweens = new();

        protected virtual int PoolSize => 8;
        protected virtual float Radius => 20f;
        protected virtual float CylinderHeight => 5f;
        protected virtual float HeightOffset => 4f;
        protected virtual float SpawnCadence => 1f;
        protected virtual Vector2 LifetimeRange => new Vector2(10f, 20f);
        
        private void Start()
        {
            _pool = new ObjectPool<GameObject>(
                OnCreateObject, 
                OnGetObject, 
                OnReleaseObject, 
                OnDestroyObject,
                false,
                PoolSize,
                PoolSize
                );
            InvokeRepeating(nameof(SpawnEffect), 0, SpawnCadence);
        }

        private void SpawnEffect()
        {
            _pool.Get();
        }

        protected virtual GameObject OnCreateObject()
        {
            var obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
            return obj;
        }

        protected virtual void OnGetObject(GameObject obj)
        {
            obj.SetActive(true);
            SetObjectAnimation(obj);
        }

        protected virtual void OnReleaseObject(GameObject obj)
        {
            if (_tweens.TryGetValue(obj, out var tween))
            {
                tween.Kill();
                _tweens.Remove(obj);
            }
            
            obj.SetActive(false);
        }

        protected void OnDestroyObject(GameObject obj)
        {
            Destroy(obj);
        }

        protected void HandleTweenComplete(GameObject obj)
        {
            if (_tweens.ContainsKey(obj))
            {
                _tweens.Remove(obj);
            }
            
            obj.SetActive(false);
            _pool.Release(obj);
        }
        
        protected virtual Vector3 GetRandomPointInCylinder(float radius, float cylinderHeight, float heightOffset)
        {
            var angle = Random.value * Mathf.PI * 2;
            var distance = Mathf.Sqrt(Random.value) * radius;
        
            return new Vector3(
                Mathf.Cos(angle) * distance,
                Random.Range(heightOffset, heightOffset + cylinderHeight),
                Mathf.Sin(angle) * distance
            );
        }

        protected virtual void SetObjectAnimation(GameObject obj)
        {
            var startPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var endPos = GetRandomPointInCylinder(Radius, CylinderHeight, HeightOffset);
            var lifeTime = Random.Range(LifetimeRange.x, LifetimeRange.y);
            obj.transform.position = startPos;
            var tween = obj.transform.TweenPosition(startPos, endPos, lifeTime)
                .SetOnComplete(() => HandleTweenComplete(obj));
            
            _tweens[obj] = tween;
        }
    }
}