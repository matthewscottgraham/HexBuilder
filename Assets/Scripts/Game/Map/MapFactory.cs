using System.Collections.Generic;
using Game.Hexes;

namespace Game.Map
{
    public class MapFactory
    {
        private readonly Dictionary<MapType, IMapStrategy> _strategies = new();

        public MapFactory()
        {
            _strategies.Add(MapType.Empty, new EmptyMap());
            _strategies.Add(MapType.Random, new RandomMap(this));
            _strategies.Add(MapType.SmallIsland, new SmallIslandMap());
            _strategies.Add(MapType.BigIsland, new BigIslandMap());
            _strategies.Add(MapType.Peninsula, new PeninsulasMap());
            _strategies.Add(MapType.Plains, new PlainsMap());
        }
        
        public List<HexInfo> CreateMap(MapType mapType)
        {
            return _strategies[mapType].GenerateMap();
        }

        public IMapStrategy GetStrategy(MapType mapType)
        {
            return _strategies[mapType];
        }
    }
}
