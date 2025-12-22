using System;
using System.Collections.Generic;
using App.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Hexes.Features
{
    public class FeatureFactory : IDisposable
    {
        private readonly Dictionary<FeatureType, FeatureModelCatalogues> _catalogues;
        private ConnectedFeatureCatalogue _riverCatalogue;
        private readonly Material _pathMaterial = Resources.Load<Material>("Materials/mat_path");
        
        public FeatureFactory()
        {
            _catalogues = GetCatalogues();
            _riverCatalogue = Resources.Load<ConnectedFeatureCatalogue>("Features/River");
        }

        public void Dispose()
        {
            _catalogues.Clear();
            _riverCatalogue = null;
        }

        public GameObject CreateFeature(FeatureType featureType)
        {
            return CreateNewFeature(featureType);
        }

        public GameObject CreateFeature(FeatureType featureType, int variation)
        {
            if (featureType == FeatureType.None) return null;
            return CreateNewFeature(featureType, false, variation);
        }
        
        public GameObject CreateVertexMesh(Vector3 vertexPosition)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(obj.GetComponent<Collider>());
            obj.name = "Vertex";
            obj.transform.position = vertexPosition;
            obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            obj.GetComponent<MeshRenderer>().material = _pathMaterial;
            return obj;
        }
        
        public GameObject GetRiverMesh(bool[] edges)
        {
            var riverPrefab = _riverCatalogue.GetRiverPrefab(edges);
            var obj = Object.Instantiate(riverPrefab.prefab);
            obj.transform.rotation = Quaternion.Euler(0, riverPrefab.rotations * 60 - 180, 0);
            obj.name = "River";
            return obj;
        }

        public GameObject CreateBridgeMesh(Vector3 vertexAPosition, Vector3 vertexBPosition)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(obj.GetComponent<Collider>());
            obj.name = "VertexBridge";
            obj.transform.position = (vertexAPosition + vertexBPosition) / 2f;
            obj.transform.rotation = Quaternion.LookRotation(vertexBPosition - vertexAPosition);
            obj.transform.localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(vertexAPosition, vertexBPosition));
            obj.GetComponent<MeshRenderer>().material = _pathMaterial;
            return obj;
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

        private GameObject CreateNewFeature(FeatureType featureType, bool getRandomPrefab = true, int prefabVariation = 0)
        {
            var (featurePrefab, variation) = _catalogues[featureType].GetPrefab(getRandomPrefab, prefabVariation);
            var instance = Object.Instantiate(featurePrefab.Prefab);
            
            if (featurePrefab.AllowRotation)
            {
                instance.transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
            }

            return instance;
        }
    }
}