using App.Services;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        public void Use(SelectionContext selection, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();

            if (!HexController.InBounds(selection.Coordinate) || hex == null) return;

            hex.SetHeight(hex.Height - 1);
        }
    }
}