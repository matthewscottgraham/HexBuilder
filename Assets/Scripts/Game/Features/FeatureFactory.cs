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
        private Dictionary<FeatureType, IObjectPool<GameObject>> _pools;

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

        public GameObject CreateFeature(FeatureType featureType)
        {
            return featureType switch
            {
                FeatureType.Mountain => _pools[FeatureType.Mountain].Get(),
                FeatureType.Wilderness => _pools[FeatureType.Wilderness].Get(),
                FeatureType.Settlement => _pools[FeatureType.Settlement].Get(),
                FeatureType.Water => CreateWater(),
                FeatureType.Path => CreatePath(),
                _ => null
            };
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
            _pools = new Dictionary<FeatureType, IObjectPool<GameObject>>
            {
                { FeatureType.Mountain, new ObjectPool<GameObject>(CreateMountain, AddRandomRotation) },
                { FeatureType.Wilderness, new ObjectPool<GameObject>(CreateWilderness, AddRandomRotation) },
                { FeatureType.Settlement, new ObjectPool<GameObject>(CreateSettlement, AddRandomRotation) }
            };
        }

        private static GameObject InstantiatePrefab(GameObject prefab)
        {
            var feature = Object.Instantiate(prefab);
            AddRandomRotation(feature);
            return feature;
        }

        private static void AddRandomRotation(GameObject feature)
        {
            var randomRotation = Random.Range(0f, 360f);
            feature.transform.localEulerAngles = new Vector3(0f, randomRotation, 0f);
        }

        private GameObject CreatePath()
        {
            var prefab = _catalogues[FeatureType.Path].GetRandomPrefab();
            return InstantiatePrefab(prefab);
        }

        private GameObject CreateWater()
        {
            var prefab = _catalogues[FeatureType.Water].GetRandomPrefab();
            return InstantiatePrefab(prefab);
        }

        private GameObject CreateSettlement()
        {
            var prefab = _catalogues[FeatureType.Settlement].GetRandomPrefab();
            return InstantiatePrefab(prefab);
        }

        private GameObject CreateWilderness()
        {
            var prefab = _catalogues[FeatureType.Wilderness].GetRandomPrefab();
            return InstantiatePrefab(prefab);
        }

        private GameObject CreateMountain()
        {
            var prefab = _catalogues[FeatureType.Mountain].GetRandomPrefab();
            return InstantiatePrefab(prefab);
        }
    }
}