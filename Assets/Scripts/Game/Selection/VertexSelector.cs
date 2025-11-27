using App.Services;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class VertexSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Vertex;
        
        protected override Transform CreateHighlighter()
        {
            var highlighter = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            highlighter.SetParent(transform);
            highlighter.localPosition = new Vector3(0, 0.125f, 0);
            highlighter.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var cell = HexGrid.GetClosestCellToPosition(worldPosition);
            var (position, vertexIndex) = HexGrid.ClampWorldPositionToVertex(worldPosition);
            position.y = HexController.GetCellHeight(cell);
            return new SelectionContext(SelectionType.Face, position, cell, null, vertexIndex);
        }
    }
}