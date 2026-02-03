using System;
using System.Collections.Generic;
using Game.Hexes;
using Random = UnityEngine.Random;

namespace Game.Map
{
    public class RandomMap : MapStrategy
    {
        private readonly MapFactory _owner;

        public RandomMap(MapFactory owner)
        {
            _owner = owner;
        }
        
        public override List<HexInfo> GenerateMap()
        {
            var strategy = _owner.GetStrategy(GetRandomMapType());
            return strategy.GenerateMap();
        }
        
        private static MapType GetRandomMapType()
        {
            var values = (MapType[])Enum.GetValues(typeof(MapType));
            return values[Random.Range(2, values.Length)]; // skip empty and random type
        }
    }
}