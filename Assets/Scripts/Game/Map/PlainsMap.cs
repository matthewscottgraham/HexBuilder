using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class PlainsMap : MapStrategy
    {
        public PlainsMap()
        {
            HeightRange = new Vector2Int(2, 4);
            Noise = new FractalBrownianMotion(50, 1);
            Falloff = new NoFalloff();
        }
    }
}