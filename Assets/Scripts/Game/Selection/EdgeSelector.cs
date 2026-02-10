using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class EdgeSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Edge;
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var cells = hexObject.Edges.GetCellsClosestToPosition(cursorPosition);
            return cells.Length == 0 ? BlankSelection : new SelectionContext(SelectionType.Edge, cells);
        }
    }
}