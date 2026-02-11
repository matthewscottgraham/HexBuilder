using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Hexes.Features
{
    [CreateAssetMenu(fileName = "NewConnectedFeatureCatalogue", menuName = "Features/New Connected Feature Catalogue")]
    public class ConnectedFeatureCatalogue : ScriptableObject
    {
        [SerializeField] private RiverPrefab[] riverPrefabs = Array.Empty<RiverPrefab>(); 
        
        public (GameObject prefab, int rotations) GetPrefab(bool[] edges)
        {
            foreach (var t in riverPrefabs)
            {
                if (GetRequiredEdgeCount(edges) != GetRequiredEdgeCount(t.Edges)) continue;
                if (GetRotation(edges, t.Edges, out var rotations))
                {
                    return (t.Prefab, rotations);
                }
            }
            //throw new Exception($"Could not find river prefab for edges: {string.Join(", ", edges)}");
            return (null, 0);
        }
        
        private static int GetRequiredEdgeCount(bool[] edges)
        {
            return edges.Count(t => t);
        }

        private static bool GetRotation(bool[] a, bool[] b, out int rotations)
        {
            var rotated = a.ToArray();
            for (rotations = 0; rotations < 6; rotations++)
            {
                if (rotated.SequenceEqual(b))
                    return true;

                rotated = RotateEdges(rotated);
            }
            return false;
        }

        private static bool[] RotateEdges(bool[] edges)
        {
            var rotated = new bool[edges.Length];
            for (var i = 0; i < edges.Length; i++)
            {
                rotated[i] = edges[(i + 1) % 6];
            }
            return rotated;
        }
    }

    [Serializable]
    public class RiverPrefab
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool[] edges = new bool[6];
        
        public GameObject Prefab => prefab;
        public bool[] Edges => edges;

        
    }
}