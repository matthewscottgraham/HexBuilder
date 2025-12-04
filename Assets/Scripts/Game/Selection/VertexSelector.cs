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
            var vertexCoordinate = HexGrid.GetClosestVertexCoordinateToPosition(worldPosition);
            var vertexPosition = HexGrid.GetVertexPosition(vertexCoordinate);
            var gridCoordinate = vertexCoordinate.GetGridCoordinate;
            vertexPosition.y = HexController.GetHexHeight(gridCoordinate);
            return new SelectionContext(SelectionType.Vertex, vertexPosition, gridCoordinate, vertexCoordinate.Z);
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 worldPosition)
        {
            var vertex = HexGrid.GetClosestVertexCoordinateToPosition(worldPosition);
            var vertexPosition = HexGrid.GetVertexPosition(vertex) + new Vector3(0, hexObject.Height, 0);
            return new SelectionContext(SelectionType.Vertex, vertexPosition, hexObject.Coordinate, vertex.Z);
        }
    }
}