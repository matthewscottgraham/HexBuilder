using App.Events;
using UnityEngine;

namespace Game.Events
{
    public struct CellSelectedEvent : IEvent
    {
        public CellSelectedEvent(Vector2Int cell)
        {
            Cell = cell;
        }
        public Vector2Int Cell;
    }
}