using System.Collections.Generic;
using System.Linq;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Map
{
    public interface IMapStrategy
    {
        public List<HexInfo> GenerateMap();

        protected static List<HexInfo> CreateBlankMap()
        {
            var hexInfos = new List<HexInfo>();
            var radius = HexGrid.GridRadius;
            for (var x = -radius; x <= radius; x++)
            {
                for (var z = Mathf.Max(-radius, -x - radius); z <= Mathf.Min(radius, -x + radius); z++)
                {
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, z), 0, FeatureType.None, 0));
                }
            }
            return hexInfos;
        }
        
        protected static Dictionary<CubicCoordinate, HexInfo> CreateHexInfoDictionary(List<HexInfo> hexInfos)
        {
            return hexInfos.ToDictionary(hexInfo => hexInfo.Coordinate);
        }
        
        protected static int DistanceFromCentre(CubicCoordinate coordinate)
        {
            return Mathf.Max(
                Mathf.Abs(coordinate.x),
                Mathf.Abs(-coordinate.x -coordinate.z),
                Mathf.Abs(coordinate.z)
            );
        }

        protected static Vector2 CubicTo2DSpace(CubicCoordinate coordinate)
        {
            return new Vector2(
                coordinate.x + coordinate.z * 0.5f,
                coordinate.z * 0.866f // This magic number is the square root of 3 / 2
            );
        }
        
        protected static float FractalBrownianMotion(Vector2 position, int octaves)
        {
            var value = 0f;
            var frequency = 1f;
            var amplitude = 1f;

            for (var i = 0; i < octaves; i++)
            {
                value += Mathf.PerlinNoise(position.x * frequency, position.y * frequency) * amplitude;
                amplitude *= 0.5f;
                frequency *= 2;
            }

            return Mathf.Clamp01(value);
        }
    }
}