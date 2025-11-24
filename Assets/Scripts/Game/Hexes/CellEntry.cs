using Game.Features;

namespace Game.Hexes
{
    public class CellEntry
    {
        public readonly Cell Cell;
        public readonly int Height;
        public readonly FeatureType? Feature;

        public CellEntry(Cell cell, int height, FeatureType? feature)
        {
            Cell = cell;
            Height = height;
            Feature = feature;
        }
    }
}