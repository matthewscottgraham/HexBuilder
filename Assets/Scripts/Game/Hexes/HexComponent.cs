using App.Utils;
using Game.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hexes
{
    public class HexComponent
    {
        protected virtual string Name => "HexComponent";
        protected HexObject Owner;
        protected Transform FeatureParent;
        protected GameObject[] Features = new GameObject[6];
        protected GameObject[] Connections = new GameObject[6];
        
        protected HexComponent(HexObject owner)
        {
            Owner = owner;
            FeatureParent = Owner.gameObject.AddChild(Name).transform;
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