using App.Services;
using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public class LevelTerrain : ITool
    {
        public float Level { get; set; }

        public void Use(SelectionContext selection, HexObject hex)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();

            if (!hexController.InBounds(selection.Cell)) return;

            if (hex == null) hex = hexController.CreateNewHex(selection.Cell);

            hex.RemoveFeature();

            hex.SetHeight(Level);
        }
    }
}