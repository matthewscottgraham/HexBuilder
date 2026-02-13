using App.Tweens;
using App.Utils;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class HexComponent
    {
        protected virtual string Name => "HexComponent";
        protected readonly HexObject Owner;
        protected readonly Transform FeatureParent;
        protected GameObject Feature;
        protected readonly bool[] HasFeatures = new bool[6];
        
        public int FeatureVariation => Owner? Owner.Variation : 0;
        public float FeatureRotation => FeatureParent.transform.localEulerAngles.y;
        public virtual FeatureType FeatureType { get; protected set; } = FeatureType.None;
        
        protected HexComponent(HexObject owner)
        {
            Owner = owner;
            FeatureParent = Owner.gameObject.AddChild(Name).transform;
        }
        
        public void SetHeight(int height)
        {
            FeatureParent.TweenLocalPosition(
                    FeatureParent.localPosition, new Vector3(0, height, 0),HexObject.AnimationDuration)
                .SetEase(HexObject.AnimationEaseType);
        }
        
        public bool Exists(int index = 0)
        {
            index %= 6;
            return HasFeatures[index];
        }
        
        public bool[] FeaturesPresent()
        {
            return HasFeatures;
        }

        public void Set(int index, bool hasFeature)
        {
            index %= 6;
            if (hasFeature)
            {
                Add(index);
            }
            else
            {
                Remove(index);
            }
        }

        protected virtual void UpdateFeatureType()
        {
            // NOOP
        }

        private void Add(int index)
        {
            if (Feature) Object.Destroy(Feature);
            HasFeatures[index] = true;
            UpdateFeatureType();
            UpdateMesh();
        }

        protected virtual void Remove(int index)
        {
            if (Feature) Object.Destroy(Feature);
            HasFeatures[index] = false;
            UpdateFeatureType();
            UpdateMesh();
        }

        protected virtual void UpdateMesh()
        {
            // NOOP
        }
    }
}