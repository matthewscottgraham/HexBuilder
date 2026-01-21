using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Map
{
    public class EmptyMap : IMapStrategy
    {
        public List<HexInfo> GenerateMap()
        {
            return IMapStrategy.CreateBlankMap();
        }
    }
}