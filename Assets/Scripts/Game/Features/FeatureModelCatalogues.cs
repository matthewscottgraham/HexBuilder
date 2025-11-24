using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Features
{
    [CreateAssetMenu(fileName = "NewFeatureModelCatalogue", menuName = "Features/New Feature Model Catalogue")]
    public class FeatureModelCatalogues : ScriptableObject
    {
        [SerializeField] private FeatureType featureType;
        [SerializeField] private GameObject[] prefabs = Array.Empty<GameObject>();

        public FeatureType FeatureType => featureType;

        public GameObject GetRandomPrefab()
        {
            var index = Random.Range(0, prefabs.Length);
            return prefabs[index];
        }
    }
}