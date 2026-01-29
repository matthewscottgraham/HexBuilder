using UnityEngine;

namespace Game.Map.Noise
{
    public interface INoise
    {
        public float GetValueAtWorldPosition(Vector3 worldPosition);
    }
}