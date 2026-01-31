using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class SmallIslandMap : MapStrategy
    {
        public SmallIslandMap()
        {
            HeightRange = new Vector2Int(0, 3);
            Noise = new FractalBrownianMotion(8, 3);
            Falloff = new RadialFalloff(4);
        }
    }
}