using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Hexes.Features
{
    public class FeatureFactory : IDisposable
    {
        private readonly Dictionary<FeatureType, FeatureModelCatalogue> _catalogues = GetCatalogues();
        private ConnectedFeatureCatalogue _riverCatalogue = Resources.Load<ConnectedFeatureCatalogue>("Features/River");
        private ConnectedFeatureCatalogue _pathCatalogue = Resources.Load<ConnectedFeatureCatalogue>("Features/Path");

        public int CurrentVariation { get; set; } = -1;
        
        public void Dispose()
        {
            _catalogues.Clear();
            _riverCatalogue = null;
            _pathCatalogue = null;
        }

        public FeatureModelCatalogue GetCatalogue(FeatureType featureType)
        {
            _catalogues.TryGetValue(featureType, out var modelCatalogue);
            return modelCatalogue;
        }

        public GameObject CreateFeature(FeatureType featureType)
        {
            return CreateNewFeature(featureType, CurrentVariation);
        }

        public GameObject CreateFeature(FeatureType featureType, int variation)
        {
            return featureType == FeatureType.None 
                ? null : 
                CreateNewFeature(featureType, variation);
        }
        
        public GameObject GetPathMesh(bool[] vertices)
        {
            var pathPrefab = _pathCatalogue.GetPrefab(vertices);
            var obj = Object.Instantiate(pathPrefab.prefab);
            obj.transform.rotation = Quaternion.Euler(0, pathPrefab.rotations * 60 + 60, 0);
            obj.name = "Path";
            return obj;
        }
        
        public GameObject GetRiverMesh(bool[] edges)
        {
            var riverPrefab = _riverCatalogue.GetPrefab(edges);
            var obj = Object.Instantiate(riverPrefab.prefab);
            obj.transform.rotation = Quaternion.Euler(0, riverPrefab.rotations * 60 - 180, 0);
            obj.name = "River";
            return obj;
        }

        private static Dictionary<FeatureType, FeatureModelCatalogue> GetCatalogues()
        {
            var modelCatalogues = Resources.LoadAll<FeatureModelCatalogue>("Features");
            var dict = new Dictionary<FeatureType, FeatureModelCatalogue>();
            foreach (var modelCatalogue in modelCatalogues)
                if (!dict.TryAdd(modelCatalogue.FeatureType, modelCatalogue))
                    Debug.LogError($"A catalogue of models for: {modelCatalogue.name} already exists");
            return dict;
        }

        private GameObject CreateNewFeature(FeatureType featureType, int prefabVariation = 0)
        {
            var featurePrefab = _catalogues[featureType].GetPrefab(prefabVariation);
            var instance = Object.Instantiate(featurePrefab.Prefab);

            if (!featurePrefab.AllowRotation) return instance;
            var rot = 60 * UnityEngine.Random.Range(0, 6);
            instance.transform.localRotation = Quaternion.Euler(0, rot, 0);

            return instance;
        }
    }
}