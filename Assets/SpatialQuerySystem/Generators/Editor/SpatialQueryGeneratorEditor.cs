using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [CustomEditor(typeof(SpatialQueryGenerator), true)]
    public class SpatialQueryGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.indentLevel++;
            DrawPropertiesExcluding(serializedObject,"m_Script");
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(10f);
        }
    }
}
