using App.Tweens;
using App.Utils;
using Game.Grid;
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
        protected readonly GameObject[] Connections = new GameObject[6];
        
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
        
        public virtual bool Exists(int index = 0)
        {
            index %= 6;
            return HasFeatures[index];
        }
        
        public bool[] FeaturesPresent()
        {
            return HasFeatures;
        }

        public virtual QuarticCoordinate? GetClosestFeatureCoordinate(Vector3 position)
        {
            return null;
        }
        
        
        public virtual void Toggle(int index = 0)
        {
            index %= 6;
            Set(index, !HasFeatures[index]);
        }

        public virtual void Set(int index, bool hasFeature)
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
            
            UpdateConnections();
            UpdateFeatureType();
        }

        protected virtual void UpdateFeatureType()
        {
            // NOOP
        }

        private void Add(int index)
        {
            if (Feature) Object.Destroy(Feature);
            HasFeatures[index] = true;
            UpdateMesh();
        }

        protected virtual void Remove(int index)
        {
            HasFeatures[index] = false;
            UpdateMesh();
        }

        protected virtual void AddConnection(int index)
        {
            // NOOP
        }
        
        protected void RemoveConnection(int index)
        {
            if (Connections[index] == null) return;
            Object.Destroy(Connections[index]);
        }

        protected virtual void UpdateConnections()
        {
            // NOOP
        }

        protected virtual void UpdateMesh()
        {
            // NOOP
        }
    }
}