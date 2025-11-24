using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.Features
{
    public class FeatureFactory : IDisposable
    {
        private readonly Dictionary<FeatureType, FeatureModelCatalogues> _catalogues;
        private Dictionary<FeatureType, IObjectPool<Feature>> _pools;

        public FeatureFactory()
        {
            _catalogues = GetCatalogues();
            CreatePools();
        }

        public void Dispose()
        {
            _catalogues.Clear();
            foreach (var pool in _pools.Values.ToArray())
            {
                pool.Clear();
            }
        }

        public Feature CreateFeature(FeatureType featureType)
        {
            return featureType switch
            {
                FeatureType.None => null,
                FeatureType.Mountain => _pools[FeatureType.Mountain].Get(),
                FeatureType.Wilderness => _pools[FeatureType.Wilderness].Get(),
                FeatureType.Settlement => _pools[FeatureType.Settlement].Get(),
                FeatureType.Water => CreateWater(),
                FeatureType.Path => CreatePath(),
                _ => null
            };
        }

        public Feature CreateFeature(FeatureType featureType, int variation, float rotation)
        {
            if (featureType == FeatureType.None) return null;
            var feature = CreateNewFeature(featureType, false, variation);
            feature.transform.localEulerAngles = new Vector3(0f, rotation, 0f);
            return feature;
        }

        private static Dictionary<FeatureType, FeatureModelCatalogues> GetCatalogues()
        {
            var modelCatalogues = Resources.LoadAll<FeatureModelCatalogues>("Features");
            var dict = new Dictionary<FeatureType, FeatureModelCatalogues>();
            foreach (var modelCatalogue in modelCatalogues)
                if (!dict.TryAdd(modelCatalogue.FeatureType, modelCatalogue))
                    Debug.LogError($"A catalogue of models for: {modelCatalogue.name} already exists");
            return dict;
        }

        private void CreatePools()
        {
            _pools = new Dictionary<FeatureType, IObjectPool<Feature>>
            {
                { FeatureType.Mountain, new ObjectPool<Feature>(() => CreateNewFeature(FeatureType.Mountain), AddRandomRotation) },
                { FeatureType.Wilderness, new ObjectPool<Feature>(() => CreateNewFeature(FeatureType.Wilderness), AddRandomRotation) },
                { FeatureType.Settlement, new ObjectPool<Feature>(() => CreateNewFeature(FeatureType.Settlement), AddRandomRotation) }
            };
        }

        private Feature CreateNewFeature(FeatureType featureType, bool getRandomPrefab = true, int prefabVariation = 0)
        {
            var (prefab, variation) = _catalogues[featureType].GetPrefab(getRandomPrefab, prefabVariation);
            var feature = InstantiatePrefab(prefab);
            feature.Initialize(featureType, variation);
            return feature;
        }

        private static Feature InstantiatePrefab(GameObject prefab)
        {
            var go = Object.Instantiate(prefab);
            var feature = go.AddComponent<Feature>();
            AddRandomRotation(feature);
            return feature;
        }

        private static void AddRandomRotation(Feature feature)
        {
            var randomRotation = Random.Range(0f, 360f);
            feature.transform.localEulerAngles = new Vector3(0f, randomRotation, 0f);
        }

        private Feature CreatePath()
        {
            var (prefab, variation) = _catalogues[FeatureType.Path].GetPrefab();
            var feature = InstantiatePrefab(prefab);
            feature.Initialize(FeatureType.Path, variation);
            return feature;
        }

        private Feature CreateWater()
        {
            var (prefab, variation) = _catalogues[FeatureType.Path].GetPrefab();
            var feature = InstantiatePrefab(prefab);
            feature.Initialize(FeatureType.Path, variation);
            return feature;
        }
    }
}