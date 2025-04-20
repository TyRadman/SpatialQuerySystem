using SpatialQuery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public partial class SpatialQueryAsset : ScriptableObject
    {
        public void Initiate_Debug(QueryDebugger debugger)
        {
            if (debugger == null)
            {
                Debug.LogError("No querier passed");
                return;
            }

            if (_selectedGenerator == null)
            {
                Debug.LogError("No _selectedGenerator");
                return;
            }

            _querier = debugger.transform;
            _samplingContext = new SpatialSamplingContext(_selectedGenerator);
            _evaluationContext = new SpatialEvaluationContext(_evaluators.ToArray());
            _evaluationContext.SetQuerier(_querier);
            _samplingContext.SetQuerier(_querier);

            _selectedGenerator.EnableDebugMode(debugger);

            for (int i = 0; i < _evaluators.Count; i++)
            {
                _evaluators[i].Init(debugger.transform);
            }
        }

        public Vector3 GetPointPosition_Debug(QueryDebugger debugger)
        {
            _samplingContext.SetSamplingPoint(_querier.position);

            RequestSpatialSampling(_samplingContext, _evaluationContext);

            for (int i = 0; i < _evaluators.Count; i++)
            {
                _evaluators[i].Evaluate(_evaluationContext);
            }

            _evaluationContext.NormalizeScores();

            Vector3 point = _evaluationContext.GetPointWithinScoreRange_Debug(SuccessScoreRange.x, SuccessScoreRange.y, GetHighestScore);

            if (DebugPoints)
            {
                DebugPointSettings settings = new DebugPointSettings()
                {
                    Duration = DebugDuration,
                    ScoreDisplayMode = ScoreDisplayMode,
                    UseDefaultGradient = !UseCustomGradientScoreColoring,
                    ScoreColorGradient = CustomGradient,
                    ScalingRange = DebugScalingRange
                };

                debugger.GenerateDebugPoints(_evaluationContext.GetSampledPoints(), settings);
            }

            _evaluationContext.ReleaseSamplePoints();
            return point;
        }
    }
}
