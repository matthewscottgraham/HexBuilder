using System;
using System.Collections.Generic;
using Game.Hexes;
using UnityEngine;

namespace Game.Tools.Paths
{
    public class PathController : MonoBehaviour, IDisposable
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private HashSet<Cell> _pathVertices = new();
        
        public void Initialize()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        public void Dispose()
        {
            _meshFilter = null;
            _meshRenderer = null;
            _pathVertices.Clear();
        }

        public void AddVertex(Cell cell, int vertexIndex)
        {
            var vertexCoordinate = GetVertexCoordinate(cell, vertexIndex);
            if (_pathVertices.Contains(vertexCoordinate)) return;
            _pathVertices.Add(vertexCoordinate);
            UpdateMesh();
        }

        public void RemoveVertex(Cell cell, int vertexIndex)
        {
            var vertexCoordinate = GetVertexCoordinate(cell, vertexIndex);
            if (!_pathVertices.Contains(vertexCoordinate)) return;
            _pathVertices.Remove(vertexCoordinate);
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            // clear mesh
            // foreach vertex create hex shape
            // for each edge between vertices create bridge
        }

        private static Cell GetVertexCoordinate(Cell cell, int vertexIndex)
        {
            var x = cell.X * 2 + Mathf.Abs(vertexIndex - 4);
            var y = cell.Y * 2;
            if (vertexIndex is 0 or 1 or 5) y += 1;
            return new Cell(x, y);
        }
    }
}