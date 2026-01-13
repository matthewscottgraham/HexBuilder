using System.Collections;
using System.Collections.Generic;
using App.Tweens;
using UnityEngine;

namespace Game.Weather
{
    public class WindController : MonoBehaviour
    {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        [SerializeField] private GameObject[] windPrefabs;
        private const float Radius = 20f;
        private const float CylinderHeight = 5;
        private const int WindAmount = 8;
        private Quaternion _windRotation;
        private readonly Queue<Transform> _windTransforms = new();
        private Dictionary<Transform, Tween<float>[]> _windTweens = new();
        
        private void Start()
        {
            _windRotation = Quaternion.Euler(new Vector3(30, Random.Range(0, 360), 0));
        
            for (var i = 0; i < WindAmount; i++)
            {
                var windTransform = Instantiate(windPrefabs[Random.Range(0, windPrefabs.Length)], transform).transform;
                var meshRenderer = windTransform.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material = new Material(meshRenderer.sharedMaterial);
                _windTransforms.Enqueue(windTransform);
            }
            
            InvokeRepeating(nameof(ShowWind), 0, 2);
        }

        private static Vector3 GetRandomPointInCylinder(float radius, float cylinderHeight, float heightOffset)
        {
            var angle = Random.value * Mathf.PI * 2;
            var distance = Mathf.Sqrt(Random.value) * radius;
        
            return new Vector3(
                Mathf.Cos(angle) * distance,
                Random.Range(heightOffset, heightOffset + cylinderHeight),
                Mathf.Sin(angle) * distance
            );
        }

        private void ShowWind()
        {
            if (_windTransforms.Count == 0) return;
            
            var windTransform = _windTransforms.Dequeue();
                
            windTransform.localPosition = GetRandomPointInCylinder(Radius, CylinderHeight, 4);
            windTransform.localRotation = _windRotation;
            windTransform.localScale = Vector3.one * Random.Range(0.8f, 1.6f);
            
            var material = windTransform.GetComponentInChildren<Renderer>().material;
            
            material.SetFloat(Alpha, 0);
            
            if (!_windTweens.ContainsKey(windTransform)) 
                _windTweens[windTransform] = new Tween<float>[2];
            
            _windTweens[windTransform][0]?.Kill();
            _windTweens[windTransform][1]?.Kill();
                
            _windTweens[windTransform][0] = material.TweenAlpha(0, 1, 1);
            _windTweens[windTransform][1] = material.TweenAlpha(1, 0, 1)
                .SetDelay(1)
                .SetOnComplete(_ => ReEnqueueWind(windTransform));
        }

        private void ReEnqueueWind(Transform windTransform)
        {
            _windTransforms.Enqueue(windTransform);
        }
    }
}
