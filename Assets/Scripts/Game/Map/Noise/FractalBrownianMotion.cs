using UnityEngine;

namespace Game.Map.Noise
{
    public class FractalBrownianMotion : INoise
    {
        private readonly float _scale;
        private readonly Vector3 _offset = new (Random.value * 1000, 0, Random.value * 1000);
        private readonly int _octaves;

        public FractalBrownianMotion(float scale, int octaves)
        {
            _scale = scale;
            _octaves = octaves;
        }
        
        public float GetValueAtWorldPosition(Vector3 worldPosition)
        {
            worldPosition += _offset;
            
            var value = 0f;
            var frequency = _scale;
            var amplitude = 1f;

            for (var i = 0; i < _octaves; i++)
            {
                value += Mathf.PerlinNoise(worldPosition.x * frequency, worldPosition.z * frequency) * amplitude;
                amplitude *= 0.5f;
                frequency *= 2;
            }

            return Mathf.Clamp01(value);
        }
    }
}