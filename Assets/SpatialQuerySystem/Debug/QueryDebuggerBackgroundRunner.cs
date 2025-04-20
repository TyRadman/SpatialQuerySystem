using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [InitializeOnLoad]
    public static class QueryDebuggerBackgroundRunner
    {
        private static float _stationaryTime;
        private const float ThresholdTime = 0.1f;
        private static QueryDebugger _debugger;

        static QueryDebuggerBackgroundRunner()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (_debugger == null)
            {
                _debugger = GameObject.FindObjectOfType<QueryDebugger>();
            }

            if (_debugger == null || !_debugger.UpdateMode)
            {
                return;
            }

            _stationaryTime += Time.deltaTime;

            if (_stationaryTime >= ThresholdTime)
            {
                _stationaryTime = 0f;

                _debugger.DebugSamplePoints();
            }
        }
    }

}
