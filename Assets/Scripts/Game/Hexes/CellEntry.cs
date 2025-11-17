namespace Game.Hexes
{
    public class CellEntry
    {
        public CellEntry(Cell cell, int height)
        {
            Cell = cell;
            Height = height;
        }
        
        public Cell Cell;
        public int Height;
    }
}