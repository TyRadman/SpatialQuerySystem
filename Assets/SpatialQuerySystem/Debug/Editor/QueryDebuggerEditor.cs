using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;

namespace SpatialQuery
{
    [CustomEditor(typeof(QueryDebugger))]
    public class QueryDebuggerEditor : Editor
    {
        private float _stationaryTime;
        private const float ThresholdTime = 0.1f;

        private QueryDebugger _debugger;
        private Editor _assetEditor;

        private void OnEnable()
        {
            _debugger = (QueryDebugger)target;
            _debugger.Init();
            _debugger.IsInitiated = true;

            _stationaryTime = 0f;

            //EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            _debugger.IsInitiated = false;
            //EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            //if (!_debugger.UpdateMode || _debugger == null)
            //{
            //    return;
            //}

            //_stationaryTime += Time.deltaTime;

            //if(_stationaryTime >= ThresholdTime)
            //{
            //    _stationaryTime = 0f;
            //    _debugger.DebugSamplePoints();
            //    return;
            //}
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _debugger.UpdateMode = EditorGUILayout.Toggle("Update Mode", _debugger.UpdateMode);

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Points"))
            {
                _debugger.DebugSamplePoints();
            }

            if (GUILayout.Button("Clear Debug Points"))
            {
                _debugger.ClearDebugPoints();
            }

            GUILayout.EndHorizontal();


            if (_assetEditor == null && _debugger.Asset != null)
            {
                _assetEditor = CreateEditor(_debugger.Asset);
            }

            if (_assetEditor != null)
            {
                EditorGUILayout.Space(20f);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space(20f);
                GUIStyle bigLabel = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                };

                EditorGUILayout.LabelField("Spatial Query Asset Properties", bigLabel);
                EditorGUILayout.Space(30f);
                _assetEditor.OnInspectorGUI();
                GUILayout.EndVertical();
            }

        }
    }
}
