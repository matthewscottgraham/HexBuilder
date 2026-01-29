using UnityEngine;

namespace Game.Map.Falloff
{
    public class NoFalloff : IFalloff
    {
        public float GetFalloffAtWorldPosition(Vector3 worldPosition)
        {
            return 1;
        }
    }
}