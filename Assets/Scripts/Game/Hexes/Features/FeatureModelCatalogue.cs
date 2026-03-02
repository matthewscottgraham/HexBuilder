using System;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Game.Hexes.Features
{
    [CreateAssetMenu(fileName = "NewFeatureModelCatalogue", menuName = "Features/New Feature Model Catalogue")]
    public class FeatureModelCatalogue : ScriptableObject
    {
        [SerializeField] private FeatureType featureType;
        [SerializeField] private FeaturePrefab[] prefabs = Array.Empty<FeaturePrefab>();

        public FeatureType FeatureType => featureType;
        public int Count => prefabs.Length;

#if UNITY_EDITOR
        [ContextMenu("Generate Icons")]
        public void GenerateIcons()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var directory = Path.GetDirectoryName(assetPath);
            var iconFolder = Path.Combine(directory, "BakedIcons");

            if (!AssetDatabase.IsValidFolder(iconFolder))
            {
                AssetDatabase.CreateFolder(directory, "BakedIcons");
            }

            foreach (var prefab in prefabs)
            {
                if (prefab.Prefab == null)
                    continue;

                var icon = GenerateIcon(prefab.Prefab);

                var filePath = Path.Combine(iconFolder, prefab.Prefab.name + ".png");
                File.WriteAllBytes(filePath, icon.EncodeToPNG());

                DestroyImmediate(icon);

                var relativePath = filePath.Replace(Application.dataPath, "Assets");

                AssetDatabase.ImportAsset(relativePath);
                var importedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

                prefab.SetIcon(importedTexture);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        
        public FeaturePrefab GetPrefab(int variation)
        {
            if (variation < 0) return GetRandomPrefab().Item1;
            variation %= prefabs.Length;
            return prefabs[variation];
        }
        
        public (FeaturePrefab, int) GetPrefab(bool useRandom = true, int variation = -1)
        {
            if (useRandom || variation < 0) return GetRandomPrefab();
        
            variation %= prefabs.Length;
            return (prefabs[variation], variation);
        }
        private (FeaturePrefab, int) GetRandomPrefab()
        {
            var index = Random.Range(0, prefabs.Length);
            return (prefabs[index], index);
        }
        
        private static Texture2D GenerateIcon(GameObject prefab, int size = 256)
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            instance.transform.position = Vector3.up * -100;

            var camera = new GameObject("IconCamera").AddComponent<Camera>();
            camera.backgroundColor = Color.clear;
            camera.clearFlags = CameraClearFlags.Color;
            camera.orthographic = false;

            var light = new GameObject("IconLight").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, 50, 0);

            var bounds = CalculateBounds(instance);
            camera.transform.position = bounds.center + new Vector3(0, 2, -bounds.extents.magnitude * 2);
            camera.transform.LookAt(bounds.center);

            var rt = new RenderTexture(size, size, 24);
            camera.targetTexture = rt;
            camera.Render();

            RenderTexture.active = rt;
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            texture.Apply();

            UnityEngine.Object.DestroyImmediate(instance);
            UnityEngine.Object.DestroyImmediate(camera.gameObject);
            UnityEngine.Object.DestroyImmediate(light.gameObject);
            RenderTexture.active = null;

            return texture;
        }
        
        private static Bounds CalculateBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;

            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            return bounds;
        }
    }
}