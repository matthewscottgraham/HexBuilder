using UnityEditor;
using UnityEngine;

namespace App.Utils.Editor
{
    [CustomEditor(typeof(MeshCombiner))]
    public class MeshCombinerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!GUILayout.Button("Combine Children")) return;
            var combiner = (MeshCombiner)target;
            combiner.CombineChildren();
        }
    }
}
