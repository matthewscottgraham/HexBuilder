using Game.Hexes;
using Game.Selection;

namespace Game.Tools
{
    public interface ITool
    {
        public bool AllowAreaOfEffect => true;
        public SelectionType SelectionType => SelectionType.Face;
        public void Use(Cell cell, HexObject hex);
    }
}