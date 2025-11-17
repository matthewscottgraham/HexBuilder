using App.Events;
using Game.Hexes;
using UnityEngine;

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