using App.Services;
using Game.Grid;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class LevelTerrain : ITool
    {
        public bool CreateHexesAsNeeded => true;
        public int Level { get; set; }

        public void Use(SelectionContext selection, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();

            if (!HexGrid.InBounds(selection.Coordinate)) return;

            if (hex == null) hex = hexController.CreateNewHex(selection.Coordinate);

            hex.SetHeight(Level);
        }
    }
}