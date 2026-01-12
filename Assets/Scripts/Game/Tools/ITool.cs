using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public interface ITool
    {
        public bool CreateHexesAsNeeded => false;
        public bool UseRadius => true;
        public SelectionType SelectionType => SelectionType.Face;
        public void Use(SelectionContext selection, HexObject hex);
    }
}