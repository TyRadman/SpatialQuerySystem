using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    [CreateAssetMenu(fileName = "SQA_NAME", menuName = "Spatial Query System/ Spatial Query Asset")]
    public partial class SpatialQueryAsset : ScriptableObject
    {
        public bool DebugPoints = true;
        public float DebugDuration = 3f;
        public ScoreDisplayMode ScoreDisplayMode = ScoreDisplayMode.ShowScoreText;
        public Vector2 DebugScalingRange = new Vector2(0.25f, 1.5f);
        public bool UseCustomGradientScoreColoring = false;
        public Gradient CustomGradient;

        public bool GetHighestScore = true;
        public Vector2 SuccessScoreRange = new(0.9f, 1.0f);
        public int SelectedIndex = 0;

        //[SerializeReference]
        public SpatialQueryGenerator _selectedGenerator;

        [HideInInspector] public List<SpatialQueryEvaluator> _evaluators = new List<SpatialQueryEvaluator>();

        private SpatialSamplingContext _samplingContext;
        private SpatialEvaluationContext _evaluationContext;

        private Transform _querier;
        private Transform _target;

        public void Initiate(Transform querier)
        {
            if (querier == null)
            {
                Debug.LogError("No querier passed");
                return;
            }

            if(_selectedGenerator == null)
            {
                Debug.LogError("No _selectedGenerator");
                return;
            }

            _querier = querier;
            _samplingContext = new SpatialSamplingContext(_selectedGenerator);
            _evaluationContext = new SpatialEvaluationContext(_evaluators.ToArray());
            _evaluationContext.SetQuerier(_querier);
            _samplingContext.SetQuerier(_querier);

            for (int i = 0; i < _evaluators.Count; i++)
            {
                _evaluators[i].Init(querier);
            }
        }

        public Vector3 GetPointPosition()
        {
            var system = SpatialQuerySystem.GetInstance();

            _samplingContext.SetSamplingPoint(_querier.position);

            RequestSpatialSampling(_samplingContext, _evaluationContext);

            for (int i = 0; i < _evaluators.Count; i++)
            {
                _evaluators[i].Evaluate(_evaluationContext);
            }

            _evaluationContext.NormalizeScores();

            Vector3 point = _evaluationContext.GetPointWithinScoreRange(SuccessScoreRange.x, SuccessScoreRange.y, GetHighestScore);

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

                system.GenerateDebugPoints(_evaluationContext.GetSampledPoints(), settings);
            }

            _evaluationContext.ReleaseSamplePoints();
            return point;
        }

        private void RequestSpatialSampling(SpatialSamplingContext context, params SpatialEvaluationContext[] evaluationContexts)
        {
            if (context == null)
            {
                Debug.LogError("No samplingContext passed");
                return;
            }

            List<SpatialQueryGenerator> generators = context.GetGenerators();

            if (generators == null || generators.Count == 0)
            {
                Debug.LogError("No generatorSettings passed");
                return;
            }

            List<SpatialQuerySamplePoint> data = new List<SpatialQuerySamplePoint>();

            for (int i = 0; i < generators.Count; ++i)
            {
                SpatialQueryGenerator generator = generators[i];
                data = generator.GenerateSamplePoints(context);
            }

            for (int i = 0; i < evaluationContexts.Length; i++)
            {
                evaluationContexts[i].SetSamplePoints(data);
            }
        }

        public void AddEvaluator(SpatialQueryEvaluator newEvaluator)
        {
            _evaluators.Add(newEvaluator);
        }

        public List<SpatialQueryEvaluator> GetEvaluators()
        {
            return _evaluators;
        }

        public void RemoveEvaluator(SpatialQueryEvaluator evaluatorToRemove)
        {
            _evaluators.Remove(evaluatorToRemove);
        }

        public void SetSelectedGenerator(SpatialQueryGenerator generator)
        {
            _selectedGenerator = generator;
        }

        public SpatialQueryEvaluator GetEvaluatorByName(string name)
        {
            return _evaluators[Random.Range(0, _evaluators.Count)];
        }

        public void SwapEvaluators(int indexA, int indexB)
        {
            (_evaluators[indexA], _evaluators[indexB]) = (_evaluators[indexB], _evaluators[indexA]);
        }

        public void UpdateTarget(Transform target)
        {
            if(target == null)
            {
                Debug.LogError("No target passed");
                return;
            }

            _target = target;
            _evaluationContext.SetTarget(_target);
            _samplingContext.SetTarget(_target);
        }

        public SpatialQueryGenerator GetSelectedGenerator()
        {
            return _selectedGenerator;
        }
    }
}
