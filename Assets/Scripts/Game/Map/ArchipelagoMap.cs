using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class ArchipelagoMap : MapStrategy
    {
        public ArchipelagoMap()
        {
            HeightRange = new Vector2Int(0, 3);
            Noise = new FractalBrownianMotion(0.6f, 3);
            Falloff = new VoronoiFalloff(12,24);
        }
    }
}