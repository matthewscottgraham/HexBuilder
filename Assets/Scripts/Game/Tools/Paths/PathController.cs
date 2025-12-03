using App.Services;
using Game.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tools.Paths
{
    public class PathController : MonoBehaviour, IDisposable
    {
        private Material _material;
        private HexGrid _hexGrid;

        private readonly Dictionary<Coordinate3, GameObject> _vertexObjects = new();
        private readonly Dictionary<Connection, GameObject> _connectionObjects = new();
        
        public void Initialize()
        {
            _hexGrid = ServiceLocator.Instance.Get<HexGrid>(); 
            _material = Resources.Load<Material>("Materials/mat_path");
        }

        public void Dispose()
        {
            foreach (var obj in _vertexObjects.Values)
            {
                Destroy(obj);
            }
            _vertexObjects.Clear();

            foreach (var obj in _connectionObjects.Values)
            {
                Destroy(obj);
            }
            _connectionObjects.Clear();
        }

        public void TogglePathOnVertex(Coordinate2 coordinate, int vertexIndex)
        {
            var vertex = HexGrid.GetVertexCoordinate(coordinate, vertexIndex);

            if (_vertexObjects.ContainsKey(vertex))
            {
                RemoveVertex(vertex);
                return;
            }
            
            _vertexObjects[vertex] = CreateVertexMesh(vertex);
            CreateConnectionsForVertex(vertex);
        }

        private void RemoveVertex(Coordinate3 vertex)
        {
            if (!_vertexObjects.TryGetValue(vertex, out var vertexObject)) return;
            
            Destroy(vertexObject);
            _vertexObjects.Remove(vertex);
            RemoveInvalidConnectionsForVertex(vertex);
        }
        
        private void CreateConnectionsForVertex(Coordinate3 vertex)
        {
            foreach (var other in _vertexObjects.Keys)
            {
                if (other.Equals(vertex)) continue;
                if (!_hexGrid.AreVerticesAdjacent(vertex, other)) continue;
                
                var connection = Connection.SetVertexOrder(vertex, other);
                if (_connectionObjects.ContainsKey(connection)) continue;
                _connectionObjects.Add(connection, CreateBridgeMesh(connection));
            }
        }

        private void RemoveInvalidConnectionsForVertex(Coordinate3 vertex)
        {
            var invalidConnections = new List<Connection>();
            foreach (var connection in _connectionObjects.Keys)
            {
                if (connection.A.Equals(vertex) && !_vertexObjects.ContainsKey(connection.B))
                {
                    invalidConnections.Add(connection);
                }
                else if (connection.B.Equals(vertex) && !_vertexObjects.ContainsKey(connection.A))
                {
                    invalidConnections.Add(connection);
                }
                else if (!_vertexObjects.ContainsKey(connection.A) || !_vertexObjects.ContainsKey(connection.B))
                {
                    invalidConnections.Add(connection);
                }
            }
            
            foreach (var connection in invalidConnections)
            {
                Destroy(_connectionObjects[connection]);
                _connectionObjects.Remove(connection);
            }
        }

        private GameObject CreateVertexMesh(Coordinate3 vertex)
        {
            // TODO: this should create a mesh rather than an object
            // also remove the hard coded height
            
            var worldPos = _hexGrid.GetVertexPosition(vertex);
            
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = $"Vertex_{vertex.X}_{vertex.Y}_{vertex.Z}";
            obj.transform.SetParent(transform);
            obj.transform.position = worldPos + new Vector3(0, 1.5f, 0);
            obj.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            obj.GetComponent<MeshRenderer>().material = _material;

            return obj;
        }

        private GameObject CreateBridgeMesh(Connection connection)
        {
            // TODO: this should create a mesh rather than an object
            // also, remove the hardcoded height
            
            var vertexAPosition = _hexGrid.GetVertexPosition(connection.A) + new Vector3(0, 2, 0);
            var vertexBPosition = _hexGrid.GetVertexPosition(connection.B) + new Vector3(0, 2, 0);

            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = $"Bridge_{connection.A.X}_{connection.A.Y}_{connection.A.Z} to {connection.B.X}_{connection.B.Y}_{connection.B.Z}";
            obj.transform.SetParent(transform);
            obj.transform.position = (vertexAPosition + vertexBPosition) / 2f;
            obj.transform.rotation = Quaternion.LookRotation(vertexBPosition - vertexAPosition);
            obj.transform.localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(vertexAPosition, vertexBPosition));

            obj.GetComponent<MeshRenderer>().material = _material;

            return obj;
        }
    }
}
