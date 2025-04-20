using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialQuery
{
    [SelectionBase]
    public class QueryDebugger : MonoBehaviour
    {
        public SpatialQueryAsset Asset;

        [SerializeField] private Transform _target;

        [Header("Defaults")]
        [SerializeField] private SpatialQueryDebugPoint _debugPointPrefab;
        [SerializeField] private Transform _pointsParent;

        [HideInInspector] public bool UpdateMode = false;

        private List<SpatialQueryDebugPoint> _debugPoints = new List<SpatialQueryDebugPoint>();
        private List<SpatialQuerySamplePoint> _samplePoints = new List<SpatialQuerySamplePoint>();

        [HideInInspector] public bool IsInitiated = false;

        private SpatialQueryGenerator _currentGenerator;

        public void Init()
        {
            if(Asset == null)
            {
                return;
            }

            Asset.Initiate_Debug(this);
            _currentGenerator = Asset.GetSelectedGenerator();
        }

        public void DebugSamplePoints()
        {
            if (Asset == null)
            {
                return;
            }

            if(!IsInitiated || _currentGenerator != Asset.GetSelectedGenerator())
            {
                Init();
                IsInitiated = true;
            }

            Asset.UpdateTarget(_target);
            Asset.GetPointPosition_Debug(this);
        }

        public void GenerateDebugPoints(List<SpatialQuerySamplePoint> points, DebugPointSettings commonSettings)
        {
            if (_debugPointPrefab == null)
            {
                _debugPointPrefab = SpatialQueryEditorAssets.LoadDebugSamplePointPrefab();
            }

            _debugPoints.RemoveAll(p => p == null);
            _debugPoints.ForEach(p => p.Hide_Debug());

            int count = _debugPoints.Count(d => !d.IsActive);

            if (count < points.Count)
            {
                for (int i = 0; i < points.Count - count; i++)
                {
                    SpatialQueryDebugPoint debugPoint = Instantiate(_debugPointPrefab, Vector3.zero, Quaternion.identity, _pointsParent);
                    _debugPoints.Add(debugPoint);
                }
            }

            var availablePoints = _debugPoints.FindAll(d => !d.IsActive);

            for (int i = 0; i < points.Count; i++)
            {
                DebugPointSettings settings = new DebugPointSettings()
                {
                    Point = points[i],
                    ScoreDisplayMode = commonSettings.ScoreDisplayMode,
                    UseDefaultGradient = commonSettings.UseDefaultGradient,
                    ScoreColorGradient = commonSettings.ScoreColorGradient,
                    Animate = false,
                    HidePointsAfterShowing = false,
                    ScalingRange = commonSettings.ScalingRange
                };

                availablePoints[i].transform.position = points[i].PointPosition;
                availablePoints[i].SetValue(settings);
            }
        }

        public SpatialQuerySamplePoint RequestSamplePoint()
        {
            _samplePoints.RemoveAll(p => p == null);

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

        public void ClearDebugPoints()
        {
            int count = _pointsParent.childCount;

            for (int i = count - 1; i >= 0; i--)
            {
                DestroyImmediate(_pointsParent.GetChild(i).gameObject);
            }
        }
    }
}
