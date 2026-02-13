using System.Collections.Generic;
using System.Linq;
using App.Tweens;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Game.Weather
{
    public class WeatherController : MonoBehaviour
    {
        [SerializeField] protected GameObject[] prefabs;
        
        private ObjectPool<GameObject> _pool;
        protected readonly Dictionary<GameObject, ITween> Tweens = new();

        protected virtual int PoolSize => 8;
        protected virtual float Radius => 20f;
        protected virtual float CylinderHeight => 5f;
        protected virtual float HeightOffset => 4f;
        protected virtual float SpawnCadence => 1f;
        protected virtual Vector2 LifetimeRange => new (10f, 20f);
        
        private void Start()
        {
            _pool = new ObjectPool<GameObject>(
                OnCreateObject, 
                OnGetObject, 
                OnReleaseObject, 
                OnDestroyObject,
                true,
                PoolSize,
                PoolSize
                );
            InvokeRepeating(nameof(SpawnEffect), 0, SpawnCadence);
        }

        private void OnDestroy()
        {
            CancelInvoke();
            foreach (var pair in Tweens.ToArray())
            {
                pair.Value.Kill();
                _pool.Release(pair.Key);
            }
        }

        private void SpawnEffect()
        {
            if (_pool.CountActive < PoolSize)
                _pool.Get();
        }

        private GameObject OnCreateObject()
        {
            var obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
            var renderableChildren = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (var child in renderableChildren)
            {
                child.gameObject.layer = 6;
            }
            return obj;
        }

        protected virtual void OnGetObject(GameObject obj)
        {
            obj.SetActive(true);
            SetObjectAnimation(obj);
        }

        private void OnReleaseObject(GameObject obj)
        {
            if (Tweens.TryGetValue(obj, out var tween))
            {
                tween.Kill();
                Tweens.Remove(obj);
            }
            
            obj.SetActive(false);
        }

        private void OnDestroyObject(GameObject obj)
        {
            Destroy(obj);
        }

        protected void HandleTweenComplete(GameObject obj)
        {
            if (Tweens.ContainsKey(obj))
            {
                Tweens.Remove(obj);
            }
            
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
            
            Tweens[obj] = tween;
        }
    }
}