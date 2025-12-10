using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        public bool CreateHexesAsNeeded => true;
        
        public void Use(SelectionContext selection, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();

            if (!HexGrid.InBounds(selection.Coordinate)) return;

            if (hex == null)
            {
                hexController.CreateNewHex(selection.Coordinate);
                return;
            }

            hex.SetHeight(hex.Height + 1);
        }
    }
}