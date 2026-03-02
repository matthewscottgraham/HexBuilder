using System;
using UnityEngine;

namespace Game.Hexes.Features
{
    [Serializable]
    public class FeaturePrefab
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool allowRotation = true;
        [SerializeField] private Texture2D icon = null;
        public GameObject Prefab => prefab;
        public bool AllowRotation => allowRotation;
        public Texture2D Icon  => icon;

        public void SetIcon(Texture2D newIcon)
        {
            icon = newIcon;
        }
    }
}