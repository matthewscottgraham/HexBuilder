using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Map
{
    public class EmptyMap : MapStrategy
    {
        public override List<HexInfo> GenerateMap()
        {
            return CreateBlankMap();
        }
    }
}