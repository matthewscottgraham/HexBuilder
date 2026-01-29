using UnityEngine;

namespace Game.Map.Falloff
{
    public interface IFalloff
    {
        public float GetFalloffAtWorldPosition(Vector3 worldPosition);
    }
}