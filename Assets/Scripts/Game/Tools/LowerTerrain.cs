using Game.Grid;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        public void Use(SelectionContext selection, HexObject hex)
        {
            if (!HexGrid.InBounds(selection.Coordinate) || hex == null) return;
            hex.SetHeight(hex.Height - 1);
        }
    }
}