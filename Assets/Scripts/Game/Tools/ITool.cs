using Game.Hexes;

namespace Game.Tools
{
    public interface ITool
    {
        public void Use(Cell cell, HexObject hex);
    }
}