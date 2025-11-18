using App.Events;
using Game.Hexes;

namespace Game.Events
{
    public struct CellSelectedEvent : IEvent
    {
        public CellSelectedEvent(Cell cell)
        {
            Cell = cell;
        }
        public Cell Cell;
    }
}