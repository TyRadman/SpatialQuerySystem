using UnityEngine;

namespace SpatialQuery
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides functionality for generating spatial sample points based on customizable parameters.
    /// </summary>
    public class SpatialQuerySystem : MonoBehaviour
    {
        [SerializeField] private SpatialQueryDebugPoint _debugPointPrefab;

        private List<SpatialQueryDebugPoint> _debugPoints = new List<SpatialQueryDebugPoint>();
        private List<SpatialQuerySamplePoint> _samplePoints = new List<SpatialQuerySamplePoint>();

        private static SpatialQuerySystem _instance;
        private static SpatialQuerySystem _debugInstance;

        private void Awake()
        {
            _instance = this;
        }

        public static SpatialQuerySystem GetInstance()
        {
            if (_instance == null)
            {
                GameObject system = new GameObject("SpatialQuerySystem_Object");
                _instance = system.AddComponent<SpatialQuerySystem>();
            }

            return _instance;
        }

        public void GenerateDebugPoints(List<SpatialQuerySamplePoint> points, DebugPointSettings commonSettings)
        {
            if(_debugPointPrefab == null)
            {
                _debugPointPrefab = SpatialQueryEditorAssets.LoadDebugSamplePointPrefab();
            }

            int count = _debugPoints.Count(d => !d.IsActive);

            if (count < points.Count)
            {
                for (int i = 0; i < points.Count - count; i++)
                {
                    SpatialQueryDebugPoint debugPoint = Instantiate(_debugPointPrefab, Vector3.zero, Quaternion.identity);
                    _debugPoints.Add(debugPoint);
                }
            }

            var availablePoints = _debugPoints.FindAll(d => !d.IsActive);

            for (int i = 0; i < points.Count; i++)
            {
                DebugPointSettings settings = new DebugPointSettings()
                {
                    Point = points[i],
                    Duration = commonSettings.Duration,
                    ScoreDisplayMode = commonSettings.ScoreDisplayMode,
                    UseDefaultGradient = commonSettings.UseDefaultGradient,
                    ScoreColorGradient = commonSettings.ScoreColorGradient,
                    ScalingRange = commonSettings.ScalingRange
                };

                availablePoints[i].transform.position = points[i].PointPosition;
                availablePoints[i].SetValue(settings);
            }
        }

        public SpatialQuerySamplePoint RequestSamplePoint()
        {
            for (int i = 0; i < _samplePoints.Count; i++)
            {
                if (_samplePoints[i].IsAvailable)
                {
                    _samplePoints[i].ResetPoint();
                    return _samplePoints[i];
                }
            }

            SpatialQuerySamplePoint point = new SpatialQuerySamplePoint();
            _samplePoints.Add(point);
            point.ResetPoint();
            return point;
        }
    }
}
