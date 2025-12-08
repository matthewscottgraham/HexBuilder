using System.Collections.Generic;
using Game.Features;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public readonly struct GameData
    {
        public GameData(Vector2Int gridSize, List<HexInfo> hexInfos)
        {
            Size = new PlanarCoordinate(gridSize.y, gridSize.y);
            Map = hexInfos;
            Features = new Dictionary<int, FeatureType>();
        }

        public readonly PlanarCoordinate Size;
        public readonly List<HexInfo> Map;
        public readonly Dictionary<int, FeatureType> Features;
    }
}