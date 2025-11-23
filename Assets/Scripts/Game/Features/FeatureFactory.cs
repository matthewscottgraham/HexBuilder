using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Features
{
    public class FeatureFactory : IDisposable
    {
        private readonly Dictionary<FeatureType, FeatureModelCatalogues> _catalogues;

        public FeatureFactory()
        {
            _catalogues = GetCatalogues();
        }

        public void Dispose()
        {
            _catalogues.Clear();
        }

        public GameObject CreateFeature(FeatureType featureType)
        {
            return featureType switch
            {
                FeatureType.Mountain => CreateMountain(),
                FeatureType.Wilderness => CreateWilderness(),
                FeatureType.Settlement => CreateSettlement(),
                FeatureType.Water => CreateWater(),
                FeatureType.Path => CreatePath(),
                _ => null
            };
        }

        private Dictionary<FeatureType, FeatureModelCatalogues> GetCatalogues()
        {
            var modelCatalogues = Resources.LoadAll<FeatureModelCatalogues>("Features");
            var dict = new Dictionary<FeatureType, FeatureModelCatalogues>();
            foreach (var modelCatalogue in modelCatalogues)
            {
                if(!dict.TryAdd(modelCatalogue.FeatureType, modelCatalogue))
                {
                    Debug.LogError($"A catalogue of models for: {modelCatalogue.name} already exists");
                }
            }
            return dict;
        }

        private static GameObject InstantiatePrefab(GameObject prefab)
        {
            var feature = UnityEngine.Object.Instantiate(prefab);
            AddRandomRotation(feature.transform);
            return feature;   
        }

        private static void AddRandomRotation(Transform feature)
        {
            var randomRotation = UnityEngine.Random.Range(0f, 360f);
            feature.localEulerAngles = new Vector3(0f, randomRotation, 0f);
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
