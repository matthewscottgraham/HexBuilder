using System;
using System.Collections.Generic;
using Game.Hexes;
using Random = UnityEngine.Random;

namespace Game.Map
{
    public class RandomMap : IMapStrategy
    {
        private readonly MapFactory _owner;

        public RandomMap(MapFactory owner)
        {
            _owner = owner;
        }
        
        public List<HexInfo> GenerateMap()
        {
            var strategy = _owner.GetStrategy(GetRandomMapType());
            return strategy.GenerateMap();
        }
        
        private MapType GetRandomMapType()
        {
            var values = (MapType[])Enum.GetValues(typeof(MapType));
            for (var i = 0; i < 10; i++)
            {
                var mapType = values[Random.Range(0, values.Length)];
                if (mapType != MapType.Random) return mapType;
            }
            return MapType.ManyIslands;
        }
    }
}