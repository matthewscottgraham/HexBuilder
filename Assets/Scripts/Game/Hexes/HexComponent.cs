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
        protected readonly GameObject[] Features = new GameObject[6];
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
            return Features[index];
        }
        
        public bool[] FeaturesPresent()
        {
            var features = new bool[6];
            for (var i = 0; i < Features.Length; i++)
            {
                features[i] = Exists(i);
            }
            return features;
        }

        public virtual QuarticCoordinate? GetClosestFeatureCoordinate(Vector3 position)
        {
            return null;
        }
        
        
        public virtual void Toggle(int index = 0)
        {
            index %= 6;
            Set(index, !Features[index]);
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

        protected virtual void Add(int index)
        {
            // NOOP
        }

        protected virtual void Remove(int index)
        {
            if (Features[index] == null) return;
            Object.Destroy(Features[index]);
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
    }
}