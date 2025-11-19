using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Features
{
    public class FeatureFactory
    {
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

        private GameObject CreatePath()
        {
            var feature = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.Destroy(feature.GetComponent<Collider>());
            return feature;
        }

        private GameObject CreateWater()
        {
            var feature = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.Destroy(feature.GetComponent<Collider>());
            return feature;
        }

        private GameObject CreateSettlement()
        {
            var feature = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Object.Destroy(feature.GetComponent<Collider>());
            return feature;
        }

        private GameObject CreateWilderness()
        {
            var feature = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(feature.GetComponent<Collider>());
            return feature;
        }

        private GameObject CreateMountain()
        {
            var feature = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(feature.GetComponent<Collider>());
            return feature;
        }
    }
}
