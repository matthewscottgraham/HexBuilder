using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class PeninsulasMap : MapStrategy
    {
        public PeninsulasMap()
        {
            HeightRange = new Vector2Int(0, 4);
            Noise = new FractalBrownianMotion(0.6f, 3);
            Falloff = new VoronoiFalloff(3,6);
        }
    }
}