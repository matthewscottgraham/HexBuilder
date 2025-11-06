using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexFactory
    {
        public GameObject CreateHex(Vector2Int cell, HexGrid grid, Transform parent)
        {
            var go = new GameObject(cell.ToString());
            go.transform.position = grid.GetHexCenter(cell.x, cell.y);
            go.transform.parent = parent;
            
            var filter = go.AddComponent<MeshFilter>();
            // add mesh
            
            var renderer = go.AddComponent<MeshRenderer>();
            // add material
            
            var collider = go.AddComponent<CapsuleCollider>();
            // set size

            return go;
        }
    }
}