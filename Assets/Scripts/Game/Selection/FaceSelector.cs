using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class FaceSelector : Selector
    {
        public override SelectionType SelectionType => SelectionType.Face;
        
        protected override Transform CreateHighlighter()
        {
            var highlighter = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            highlighter.SetParent(transform);
            highlighter.localPosition = new Vector3(0, 0.5f, 0);
            highlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
            return highlighter;
        }
        
        protected override SelectionContext GetClampedSelection(Vector3 worldPosition)
        {
            var hexCoordinate = HexGrid.GetClosestFaceCoordinateToPosition(worldPosition);
            var facePosition = HexGrid.GetFacePosition(hexCoordinate);
            facePosition.y = HexController.GetHexHeight(hexCoordinate);
            return new SelectionContext(SelectionType.Face, facePosition, hexCoordinate, null);
        }
        
        protected override SelectionContext GetClampedSelection(HexObject hexObject, Vector3 worldPosition)
        {
            var facePosition = hexObject.transform.position + new Vector3(0, hexObject.Height, 0);
            return new SelectionContext(SelectionType.Face, facePosition, hexObject.Coordinate, null);
        }
    }
}