using UnityEngine;

namespace Game.Features
{
    public class Feature : MonoBehaviour
    {
        public FeatureType FeatureType {get; private set;}
        public int Variation {get; private set;}

        public void Initialize(FeatureType featureType, int variation)
        {
            FeatureType = featureType;
            Variation = variation;
        }
    }
}
