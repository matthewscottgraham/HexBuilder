using Game.Grid;
using UnityEngine;

namespace Game.Map.Falloff
{
    public class VoronoiFalloff : IFalloff
    {
        private readonly Vector3[] _centres;
        
        public VoronoiFalloff(int min, int max)
        {
            _centres = new Vector3[Random.Range(min, max)];
            for (var i = 0; i < _centres.Length; i++)
            {
                var c = Random.insideUnitCircle * HexGrid.GridRadius * 0.8f;
                _centres[i] = new Vector3(c.x, 0, c.y);
            }
        }
        public float GetFalloffAtWorldPosition(Vector3 worldPosition)
        {
            var d1 = float.MaxValue;
            var d2 = float.MaxValue;

            foreach (var centre in _centres)
            {
                var distance = Vector3.Distance(worldPosition, centre);
                if (distance < d1)
                {
                    d2 = d1;
                    d1 = distance;
                }
                else if (distance < d2)
                {
                    d2 = distance;
                }
            }

            var edge = d2 - d1;
            var t = Mathf.Clamp01(edge / (HexGrid.GridRadius * 0.3f));
            return Mathf.SmoothStep(1f, 0f, t);
        }
    }
}