using System.Collections.Generic;
using Game.Hexes;

namespace Game.Map
{
    public interface IMapStrategy
    {
        public List<HexInfo> GenerateMap();
    }
}