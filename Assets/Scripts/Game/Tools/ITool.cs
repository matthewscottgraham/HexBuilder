using System.Collections.Generic;
using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public interface ITool
    {
        public Sprite Icon { get; }
        public bool CreateHexesAsNeeded => false;
        public bool UseRadius => true;
        public SelectionType SelectionType => SelectionType.Face;
        public void Use(HexObject hex);
        public void Use(HexObject[] hexes){ }
    }
}