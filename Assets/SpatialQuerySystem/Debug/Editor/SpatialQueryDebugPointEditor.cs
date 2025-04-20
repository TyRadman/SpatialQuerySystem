using SpatialQuery;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [CustomEditor(typeof(SpatialQueryDebugPoint))]
    public class SpatialQueryDebugPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var debugPoint = (SpatialQueryDebugPoint)target;

            GUILayout.Space(20f);
            EditorGUILayout.LabelField("Report", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); 
            EditorGUILayout.LabelField(debugPoint.Report, new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true, font = EditorStyles.label.font });

            EditorGUILayout.EndVertical();
        }
    }

}
