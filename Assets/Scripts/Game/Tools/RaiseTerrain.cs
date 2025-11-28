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

            if (!hexController.InBounds(selection.Cell)) return;

            if (hex == null)
            {
                hexController.CreateNewHex(selection.Cell);
                return;
            }

            hex.SetHeight(hex.Height + 1);
        }
    }
}