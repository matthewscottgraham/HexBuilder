using Game.Grid;
using UnityEngine;

namespace Game.Map.Falloff
{
    public class RadialFalloff : IFalloff
    {
        private readonly Vector3 _worldCenter = HexGrid.GetWorldPosition(new CubicCoordinate(0,0));
        private readonly float _worldRadius = HexGrid.GridRadius * HexGrid.HexRadius;
        
        public float GetFalloffAtWorldPosition(Vector3 worldPosition)
        {
            var t = Vector3.Distance(worldPosition, _worldCenter) / _worldRadius;
            t = Mathf.Clamp01(t);
            return 1f / (1f + Mathf.Exp(6f * (t - 0.6f))); // This is a S curve style falloff
        }
    }
}