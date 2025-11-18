namespace Game.Hexes
{
    public class CellEntry
    {
        public readonly Cell Cell;
        public readonly int Height;

        public CellEntry(Cell cell, int height)
        {
            Cell = cell;
            Height = height;
        }
    }
}