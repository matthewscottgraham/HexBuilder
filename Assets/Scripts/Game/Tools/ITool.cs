using Game.Hexes;

namespace Game.Tools
{
    public interface ITool
    {
        public bool AllowAreaOfEffect => true;
        public void Use(Cell cell, HexObject hex);
    }
}