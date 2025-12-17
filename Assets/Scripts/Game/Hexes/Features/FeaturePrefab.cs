using System;
using UnityEngine;

namespace Game.Hexes.Features
{
    [Serializable]
    public class FeaturePrefab
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool allowRotation = true;
        
        public GameObject Prefab => prefab;
        public bool AllowRotation => allowRotation;
    }
}