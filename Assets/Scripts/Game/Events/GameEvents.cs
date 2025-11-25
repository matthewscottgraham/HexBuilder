using App.Events;
using Game.Hexes;
using Game.Selection;

namespace Game.Events
{
    public struct SelectionEvent : IEvent
    {
        public SelectionEvent(SelectionContext selection)
        {
            Selection = selection;
        }
        
        public SelectionContext Selection;
    }

    public struct HoverEvent : IEvent
    {
        public HoverEvent(SelectionContext hoverSelection)
        {
            HoverSelection = hoverSelection;
        }
        
        public SelectionContext HoverSelection;
    }
}