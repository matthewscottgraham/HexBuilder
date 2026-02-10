using System.Collections.Generic;
using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public interface ITool
    {
        public int RadiusIncrement => 0;
        public Sprite Icon { get; }
        public bool CreateHexesAsNeeded => false;
        public bool UseRadius => true;
        public SelectionType SelectionType => SelectionType.Face;
        public bool Use(HexObject hex);
        public bool Use(HexObject[] hexes){ return false; }
    }
}