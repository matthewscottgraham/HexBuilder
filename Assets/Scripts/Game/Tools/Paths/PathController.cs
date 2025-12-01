using System;
using System.Collections.Generic;
using System.Linq;
using App.Services;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools.Paths
{
    public class PathController : MonoBehaviour, IDisposable
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private HexGrid _hexGrid;
        private Dictionary<Vector3Int, GameObject> _pathVertices = new();
        private Dictionary<Vector3Int, Vector3> _vertexPositions;


        public void Initialize()
        {
            _hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _vertexPositions = GetVertexPositions();
            
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        public void Dispose()
        {
            _meshFilter = null;
            _meshRenderer = null;
            _pathVertices.Clear();
            _vertexPositions.Clear();
        }

        public void AddVertex(Cell cell, int vertexIndex)
        {
            var vertexCoordinate = GetVertexCoordinate(cell, vertexIndex);
            if (!_pathVertices.TryAdd(vertexCoordinate, null)) return;
            UpdateMesh();
        }

        public void RemoveVertex(Cell cell, int vertexIndex)
        {
            var vertexCoordinate = GetVertexCoordinate(cell, vertexIndex);
            if (!_pathVertices.ContainsKey(vertexCoordinate)) return;
            _pathVertices.Remove(vertexCoordinate);
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            // clear mesh
            
            // foreach vertex create hex shape
            foreach (var pair in _pathVertices.ToList())
            {
                if (pair.Value != null) continue;
                _pathVertices[pair.Key] = CreateVertexMesh(pair.Key);
            }
            
            // for each edge between vertices create bridge
        }

        private static readonly Vector2Int[] CornerOwnerOffset = {
            new(0,0),  // corner 0 owned by (x,y)
            new(0,0),
            new(0,1),  // corner 2 owned by (x,y+1)
            new(-1,1), // corner 3 owned by (x-1,y+1)
            new(-1,0), // corner 4 owned by (x-1,y)
            new(0,0)
        };

        private Vector3Int GetVertexCoordinate(Cell cell, int vertexIndex)
        {
            var offset = CornerOwnerOffset[vertexIndex];
            return new Vector3Int(cell.X + offset.x, cell.Y + offset.y, vertexIndex);
        }


        private GameObject CreateVertexMesh(Vector3Int vertexCoordinate)
        {
            var worldPosition = _vertexPositions[vertexCoordinate];
            var vertexMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vertexMesh.name = "VertexMesh";
            vertexMesh.transform.SetParent(transform);
            vertexMesh.transform.localScale = new Vector3(0.5f, 3f, 0.5f);
            vertexMesh.transform.position = worldPosition;
            return vertexMesh;
        }

        private void OnDrawGizmos()
        {
            if (_vertexPositions is null)
            {
                _vertexPositions = GetVertexPositions();
                return;
            }
            Gizmos.color = Color.green;
            foreach (var pair in _vertexPositions)
            {
                Gizmos.DrawSphere(pair.Value, 0.2f);
            }
        }

        private Dictionary<Vector3Int, Vector3> GetVertexPositions()
        {
            if (_hexGrid == null) return null;

            var vertices = new Dictionary<Vector3Int, Vector3>();

            for (var x = 0; x < _hexGrid.GridSize.x; x++)
            {
                for (var y = 0; y < _hexGrid.GridSize.y; y++)
                {
                    var worldPos = _hexGrid.GetHexWorldPosition(x, y);

                    for (var corner = 0; corner < 6; corner++)
                    {
                        var offset = CornerOwnerOffset[corner];
                        var v = new Vector3Int(x + offset.x, y + offset.y, corner
                        );
                        
                        var pos = worldPos + _hexGrid.GetHexRelativeCornerPosition(corner);
                        
                        vertices.TryAdd(v, pos);
                    }
                }
            }

            return vertices;
        }
    }
}