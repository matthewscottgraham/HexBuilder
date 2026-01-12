using System.Collections;
using System.Collections.Generic;
using App.Tweens;
using UnityEngine;

namespace Game.Weather
{
    public class WindController : MonoBehaviour
    {
        [SerializeField] private GameObject[] windPrefabs;
        private const float Radius = 20f;
        private const float CylinderHeight = 5;
        private Quaternion _windRotation;
        private int _windAmount;
        
        private readonly Queue<Transform> _windTransforms = new();
        
        private void Start()
        {
            _windRotation = Quaternion.Euler(new Vector3(30, Random.Range(0, 360), 0));
            _windAmount = Random.Range(0, 15);
        
            for (var i = 0; i < _windAmount; i++)
            {
                var windTransform = Instantiate(windPrefabs[Random.Range(0, windPrefabs.Length)], transform).transform;
                windTransform.gameObject.SetActive(false);
                _windTransforms.Enqueue(windTransform);
            }

            for (var i = 0; i < _windAmount / 2; i++)
            {
                ShowWind();
            }
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
            if (_windTransforms.Count <= 0) return;
            
            var windTransform = _windTransforms.Dequeue();
            windTransform.localPosition = GetRandomPointInCylinder(Radius, CylinderHeight, 4);
            windTransform.localRotation = _windRotation;
            windTransform.localScale = Vector3.one * Random.Range(0.8f, 1.6f);
            windTransform.gameObject.SetActive(true);

            var lifeTime = Random.Range(3f, 10);
            var material = windTransform.GetComponentInChildren<Renderer>().material;
            material.TweenAlpha(1, 0, 1).SetDelay(lifeTime - 1); //.OnComplete(HideWind(windTransform));
        }

        private void HideWind(Transform windTransform)
        {
            windTransform.gameObject.SetActive(false);
            _windTransforms.Enqueue(windTransform);
            ShowWind();
        }
    }
}
