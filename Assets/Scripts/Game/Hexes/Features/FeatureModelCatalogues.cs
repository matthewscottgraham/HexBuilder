using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Hexes.Features
{
    [CreateAssetMenu(fileName = "NewFeatureModelCatalogue", menuName = "Features/New Feature Model Catalogue")]
    public class FeatureModelCatalogues : ScriptableObject
    {
        [SerializeField] private FeatureType featureType;
        [SerializeField] private GameObject[] prefabs = Array.Empty<GameObject>();

        public FeatureType FeatureType => featureType;

        public (GameObject, int) GetPrefab(bool useRandom = true, int variation = -1)
        {
            if (useRandom) return GetRandomPrefab();
            
            variation = Mathf.Clamp(variation, 0, prefabs.Length);
            return (prefabs[variation], variation);
        }
        private (GameObject, int) GetRandomPrefab()
        {
            var index = Random.Range(0, prefabs.Length);
            return (prefabs[index], index);
        }
    }
}