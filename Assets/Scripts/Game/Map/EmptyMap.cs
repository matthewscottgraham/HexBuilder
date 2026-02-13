using System.Collections.Generic;
using Game.Hexes;

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