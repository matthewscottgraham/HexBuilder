namespace Game.Hexes
{
    public class CellEntry
    {
        public CellEntry(Cell cell, int height)
        {
            Cell = cell;
            Height = height;
        }
        
        public readonly Cell Cell;
        public readonly int Height;
    }
}