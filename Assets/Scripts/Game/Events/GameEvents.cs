using App.Events;
using Game.Hexes;
using UnityEngine;

namespace Game.Events
{
    public struct CellSelectedEvent : IEvent
    {
        public CellSelectedEvent(Coordinate cell)
        {
            Cell = cell;
        }
        public Coordinate Cell;
    }
}