using App.Tweens;
using Game.Grid;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private Transform _hexMesh;
        
        public static float AnimationDuration => 0.3f;
        public static EaseType AnimationEaseType => EaseType.BounceOut;
        
        public CubicCoordinate Coordinate { get; private set; }
        public int Height { get; private set; } = 1;
        public FaceFeatures Face { get; private set; }
        public EdgeFeatures Edges { get; private set; }
        public VertexFeatures Vertices { get; private set; }
        
        public int Variation { get; private set; }
        
        public void Initialize(CubicCoordinate coordinate, Transform hexMesh)
        {
            Coordinate = coordinate;
            _hexMesh = hexMesh;
            _hexMesh.SetParent(transform, false);

            Face = new FaceFeatures(this);
            Edges = new EdgeFeatures(this);
            Vertices = new VertexFeatures(this);
        }

        public void SetHeight(int height)
        {
            if (height < 0) height = 0;
            Height = height;
            _hexMesh.TweenScale(_hexMesh.transform.localScale, new Vector3(1, height, 1), AnimationDuration)
                .SetEase(AnimationEaseType);

            Vertices.SetHeight(height);
            Edges.SetHeight(height);
            Face.SetHeight(height);
        }
        
        private void OnDrawGizmos()
        {
            // var vertices = new Vector3[6];
            // for (var i = 0; i < 6; i++)
            // {
            //     vertices[i] = GetVertexPosition(i);
            // }
            //
            // // Face
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawWireSphere(GetFacePosition(), 1.8f);
            // for (var i = 0; i < 6; i++)
            // {
            //     Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 6]);
            // }
            //
            // // Vertices
            // Gizmos.color = Color.red;
            // for (var i = 0; i < 6; i++)
            // {
            //     Gizmos.DrawWireSphere(vertices[i], 0.6f);
            // }
            //
            // // Edges
            // Gizmos.color = Color.blue;
            // for (var i = 0; i < 6; i++)
            // {
            //     var pos = Vector3.Lerp(vertices[i], vertices[(i + 1) % 6], 0.5f);
            //     Gizmos.DrawWireSphere(pos, 0.8f);
            // }
        }
    }
}