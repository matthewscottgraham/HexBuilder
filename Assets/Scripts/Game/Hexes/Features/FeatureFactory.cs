using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Hexes.Features
{
    public class FeatureFactory : IDisposable
    {
        private readonly Dictionary<FeatureType, FeatureModelCatalogues> _catalogues;
        private Dictionary<FeatureType, IObjectPool<GameObject>> _pools;
        private readonly Material _pathMaterial = Resources.Load<Material>("Materials/mat_path");
        private readonly Material _riverMaterial = Resources.Load<Material>("Materials/mat_river");
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
                _ => null
            };
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
        
        public GameObject CreateEdgeMesh(Vector3 hexCenter, Vector3 edgePosition)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(obj.GetComponent<Collider>());
            obj.name = "Edge";
            obj.transform.position = Vector3.Lerp(hexCenter, edgePosition, 0.5f);
            obj.transform.localScale = new Vector3(0.8f, 0.6f, Vector3.Distance(hexCenter, edgePosition));
            obj.transform.LookAt(hexCenter);
            obj.GetComponent<MeshRenderer>().material = _riverMaterial;
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

        private void CreatePools()
        {
            _pools = new Dictionary<FeatureType, IObjectPool<GameObject>>
            {
                { FeatureType.Mountain, new ObjectPool<GameObject>(() => CreateNewFeature(FeatureType.Mountain)) },
                { FeatureType.Wilderness, new ObjectPool<GameObject>(() => CreateNewFeature(FeatureType.Wilderness)) },
                { FeatureType.Settlement, new ObjectPool<GameObject>(() => CreateNewFeature(FeatureType.Settlement)) }
            };
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