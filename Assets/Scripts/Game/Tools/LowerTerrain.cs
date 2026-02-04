using Game.Grid;
using Game.Hexes;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/lower");
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (!HexGrid.InBounds(selection.Coordinate) || hex == null) return;
            hex.SetHeight(hex.Height - 1);
        }
    }
}