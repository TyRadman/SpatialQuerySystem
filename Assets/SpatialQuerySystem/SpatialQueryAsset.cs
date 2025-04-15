using System.Collections.Generic;
using UnityEngine;
using Radnom = UnityEngine.Random;

namespace SpatialQuery
{
    [CreateAssetMenu(fileName = "SQA_NAME", menuName = "Spatial Query System/ Spatial Query Asset")]
    public class SpatialQueryAsset : ScriptableObject
    {
        public bool DebugPoints = true;
        public bool GetHighestScore = true;
        public Vector2 SuccessScoreRange = new(0.9f, 1.0f);

        [HideInInspector] public SpatialQueryGenerator Generator;
        public int SelectedIndex = 0;
        [SerializeReference]
        public GeneratorSettings _selectedGenerator;

        public GridGeneratorSettings GridSettings = new GridGeneratorSettings();
        public RingGeneratorSettings RingSettings = new RingGeneratorSettings();
        public CircleGridGeneratorSettings CircleSettings = new CircleGridGeneratorSettings();
        public ConeGeneratorSettings ConeSettings = new ConeGeneratorSettings();

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

            system.RequestSpatialSampling(_samplingContext, _evaluationContext);

            for (int i = 0; i < _evaluators.Count; i++)
            {
                _evaluators[i].Evaluate(_evaluationContext);
            }

            _evaluationContext.NormalizeScores();

            Vector3 point = _evaluationContext.GetPointWithinScoreRange(SuccessScoreRange.x, SuccessScoreRange.y, GetHighestScore);

            if (DebugPoints)
            {
                system.GenerateDebugPoints(_evaluationContext.GetSampledPoints());
            }

            _evaluationContext.ReleaseSamplePoints();
            return point;
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

        public void SetSelectedGenerator(GeneratorSettings generator)
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
    }
}
