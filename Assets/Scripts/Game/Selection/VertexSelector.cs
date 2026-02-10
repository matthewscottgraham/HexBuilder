using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class VertexSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Vertex;
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 cursorPosition)
        {
            var cells = hexObject.Vertices.GetCellsClosestToPosition(cursorPosition);
            return cells.Length == 0 ? BlankSelection : new SelectionContext(SelectionType.Vertex, cells);
        }
    }
}