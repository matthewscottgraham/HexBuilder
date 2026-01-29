using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class BigIslandMap : MapStrategy
    {
        public BigIslandMap()
        {
            HeightRange = new Vector2Int(0, 3);
            Noise = new FractalBrownianMotion(10, 5);
            Falloff = new RadialFalloff(6);
        }
    }
}