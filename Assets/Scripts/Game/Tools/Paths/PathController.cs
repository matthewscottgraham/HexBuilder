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
        private Material _material;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private HexGrid _hexGrid;
        private Dictionary<Vector3Int, GameObject> _pathVertices = new();
        private Dictionary<Vector3Int, Vector3> _vertexPositions;


        public void Initialize()
        {
            _hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _vertexPositions = _hexGrid.GetVertexPositions();
            
            _material = Resources.Load<Material>("Materials/mat_path");
            
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
            var vertexCoordinate = HexGrid.GetVertexCoordinate(cell, vertexIndex);
            if (!_pathVertices.TryAdd(vertexCoordinate, null)) return;
            UpdateMesh();
        }

        public void RemoveVertex(Cell cell, int vertexIndex)
        {
            var vertexCoordinate = HexGrid.GetVertexCoordinate(cell, vertexIndex);
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

        


        private GameObject CreateVertexMesh(Vector3Int vertexCoordinate)
        {
            var worldPosition = _vertexPositions[vertexCoordinate];
            var vertexMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vertexMesh.name = "VertexMesh";
            vertexMesh.GetComponent<MeshRenderer>().material = _material;
            vertexMesh.transform.SetParent(transform);
            vertexMesh.transform.localScale = new Vector3(0.5f, 3f, 0.5f);
            vertexMesh.transform.position = worldPosition;
            return vertexMesh;
        }

        private void OnDrawGizmos()
        {
            if (_vertexPositions is null)
            {
                _vertexPositions = _hexGrid.GetVertexPositions();
                return;
            }
            Gizmos.color = Color.green;
            foreach (var pair in _vertexPositions)
            {
                Gizmos.DrawSphere(pair.Value, 0.2f);
            }
        }

        
    }
}