using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpatialQuery
{
    public class SpatialEvaluationContext
    {
        public Transform Querier { get; private set; }
        public Transform Target { get; private set; }
        public float HighestScore = -1f;
        public int HighestScoreIndex = -1;

        private List<SpatialQuerySamplePoint> _samplePoints = new List<SpatialQuerySamplePoint>();
        public List<SpatialQueryEvaluator> _settings = new List<SpatialQueryEvaluator>();

        public SpatialEvaluationContext(params SpatialQueryEvaluator[] evaluatorSettings)
        {
            for (int i = 0; i < evaluatorSettings.Length; i++)
            {
                _settings.Add(evaluatorSettings[i]);
            }
        }

        public void SetSamplePoints(List<SpatialQuerySamplePoint> points)
        {
            _samplePoints = points;
        }

        public List<SpatialQuerySamplePoint> GetSampledPoints()
        {
            return _samplePoints;
        }

        public List<SpatialQueryEvaluator> GetEvaluatorSettings()
        {
            return _settings;
        }

        public Vector3 GetPointWithinScoreRange_Debug(float minScore, float maxScore, bool getHighestScore)
        {
            _samplePoints.RemoveAll(p => p == null);

            if(getHighestScore && HighestScoreIndex == -1)
            {
                Debug.LogError("No highest score");
                return Vector3.zero;
            }
         
            if (_samplePoints == null || _samplePoints.Count == 0)
            {
                Debug.LogError("No sample points");
                return Vector3.zero;
            }

            SpatialQuerySamplePoint selectedPoint;

            if (getHighestScore)
            {
                selectedPoint = _samplePoints[HighestScoreIndex];
            }
            else
            {
                var qualifiedPoints = _samplePoints.FindAll(p => p.Score >= minScore && p.Score <= maxScore && !p.IsFilteredOut);
                selectedPoint = qualifiedPoints[Random.Range(0, qualifiedPoints.Count)];
            }

            if (selectedPoint == null)
            {
                Debug.LogError($"No selected point {_samplePoints.FindAll(p => p == null).Count} out of {_samplePoints.Count}. Highest score: {_samplePoints.OrderBy(p => p.Score).First().Score}");

                return Vector3.zero;
            }

            selectedPoint.IsWinner = true;
            return selectedPoint.PointPosition;
        }

        public Vector3 GetPointWithinScoreRange(float minScore, float maxScore, bool getHighestScore)
        {
            if(_samplePoints == null || _samplePoints.Count == 0)
            {
                Debug.LogError("No sample points");
                return Vector3.zero;
            }

            SpatialQuerySamplePoint selectedPoint;

            if (getHighestScore)
            {
                selectedPoint = _samplePoints[HighestScoreIndex];
            }
            else
            {
                var qualifiedPoints = _samplePoints.FindAll(p => p.Score >= minScore && p.Score <= maxScore && !p.IsFilteredOut);
                selectedPoint = qualifiedPoints[Random.Range(0, qualifiedPoints.Count)];
            }

            if(selectedPoint == null)
            {
                Debug.LogError($"No selected point {_samplePoints.FindAll(p => p == null).Count} out of {_samplePoints.Count}. Highest score: {_samplePoints.OrderBy(p => p.Score).First().Score}");

                return Vector3.zero;
            }

            selectedPoint.IsWinner = true;
            return selectedPoint.PointPosition;
        }

        public void NormalizeScores()
        {
            if (_samplePoints == null || _samplePoints.Count == 0)
            {
                return;
            }

            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (var score in _samplePoints)
            {
                if (score.IsFilteredOut)
                {
                    continue;
                }

                if (score.Score < min)
                {
                    min = score.Score;
                }

                if (score.Score > max)
                {
                    max = score.Score;
                }
            }

            if (Mathf.Approximately(min, max))
            {
                return;
            }
                
            for (int i = 0; i < _samplePoints.Count; i++)
            {
                if (_samplePoints[i].IsFilteredOut)
                {
                    continue;
                }

                FinalizeReport(_samplePoints[i], min, max);
                _samplePoints[i].Score = Mathf.InverseLerp(min, max, _samplePoints[i].Score);
            }
        }

        private void FinalizeReport(SpatialQuerySamplePoint point, float min, float max)
        {
            float finalScore = Mathf.InverseLerp(min, max, point.Score);
            point.Report.Append($"\n\n<b>Raw score</b>: {point.Score:0.00}\n<b>Min</b>: {min:0.00}, <b>Max</b>: {max:0.00}\n<b>Final score</b>: {finalScore:0.00}");
        }

        public void ReleaseSamplePoints()
        {
            for (int i = 0; i < _samplePoints.Count; ++i)
            {
                _samplePoints[i].IsAvailable = true;
            }

            HighestScore = -1f;
            HighestScoreIndex = -1;
            _samplePoints.Clear();
        }

        public void SetTarget(Transform target)
        {
            if(target == null)
            {
                Debug.LogError("No target");
                return;
            }

            Target = target;
        }

        public void SetQuerier(Transform querier)
        {
            if (querier == null)
            {
                Debug.LogError("No querier");
                return;
            }

            Querier = querier;
        }
    }
}
