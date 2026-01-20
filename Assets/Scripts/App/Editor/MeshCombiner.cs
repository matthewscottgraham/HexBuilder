using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace App.Utils
{
    public class MeshCombiner : MonoBehaviour
    {
        [ContextMenu("Combine Children Meshes")]
        public void CombineChildren()
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>()
                .Where(mf => mf.transform != transform && mf.sharedMesh != null).ToArray();
            
            if (meshFilters.Length == 0) return;
            
            var originalMeshRenderer = meshFilters[0].gameObject.GetComponent<MeshRenderer>();
            var originalMaterial = originalMeshRenderer.sharedMaterial;
            
            var combineInstances = new CombineInstance[meshFilters.Length];
        
            for (var i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
                Undo.DestroyObjectImmediate(meshFilters[i].gameObject);
            }
        
            var meshFilter = gameObject.AddChild<MeshFilter>("CombinedMeshes");
            var meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = originalMaterial;
            
            Undo.RegisterCreatedObjectUndo(meshFilter.gameObject, "Combine Children Meshes");
            
            var combinedMesh = new Mesh{ name ="CombinedMesh" };
            meshFilter.sharedMesh = combinedMesh;
            meshFilter.sharedMesh.CombineMeshes(combineInstances, true, true);
            
            CreateCombinedMeshDirectory();
            AssetDatabase.CreateAsset(combinedMesh, GetCombinedMeshAssetName());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            DestroyImmediate(this);
        }

        private string GetCombinedMeshAssetName()
        {
            var assetPath = $"Assets/Models/CombinedMeshes/{gameObject.name}_CombinedMesh.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            return assetPath;
        }

        private static void CreateCombinedMeshDirectory()
        {
            if (!Directory.Exists(Application.dataPath + "/Models"))
            {
                AssetDatabase.CreateFolder("Assets", "Models");
                AssetDatabase.Refresh();
            }

            if (!Directory.Exists(Application.dataPath + "/Models/CombinedMeshes"))
            {
                AssetDatabase.CreateFolder("Assets/Models", "CombinedMeshes");
                AssetDatabase.Refresh();
            }
        }
    }
}
