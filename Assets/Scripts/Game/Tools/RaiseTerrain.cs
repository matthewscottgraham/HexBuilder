using App.Services;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        public void Use(SelectionContext selection, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();

            if (!hexController.InBounds(selection.Coordinate)) return;

            if (hex == null)
            {
                hexController.CreateNewHex(selection.Coordinate);
                return;
            }

            hex.SetHeight(hex.Height + 1);
        }
    }
}