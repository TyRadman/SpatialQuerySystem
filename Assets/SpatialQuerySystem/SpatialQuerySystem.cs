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

        private Dictionary<System.Type, SpatialQueryGenerator> _cachedGenerators;

        private List<SpatialQueryDebugPoint> _debugPoints = new List<SpatialQueryDebugPoint>();
        private List<SpatialQuerySamplePoint> _samplePoints = new List<SpatialQuerySamplePoint>();

        public bool IsActive { get; private set; }

        private static SpatialQuerySystem _instance;

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

        private void OnEnable()
        {
            _cachedGenerators = new Dictionary<System.Type, SpatialQueryGenerator>()
            {
                {typeof(CircleGridGeneratorSettings), new SpatialQueryCircleGridGenerator()},
                {typeof(GridGeneratorSettings), new GridGenerator()},
                {typeof(RingGeneratorSettings), new RingGenerator()},
                {typeof(ConeGeneratorSettings), new SpatialQueryConeGenerator()}
            };
        }

        public void RequestSpatialSampling(SpatialSamplingContext context, params SpatialEvaluationContext[] evaluationContexts)
        {
            if (context == null)
            {
                Debug.LogError("No samplingContext passed");
                return;
            }

            List<GeneratorSettings> generators = context.GetGenerators();

            if (generators == null || generators.Count == 0)
            {
                Debug.LogError("No generatorSettings passed");
                return;
            }

            List<SpatialQuerySamplePoint> data = new List<SpatialQuerySamplePoint>();

            for (int i = 0; i < generators.Count; ++i)
            {
                GeneratorSettings settings = generators[i];

                if(_cachedGenerators.TryGetValue(settings.GetType(), out SpatialQueryGenerator generator))
                {
                    data = generator.GenerateSamplePoints(settings, context);
                }
                else
                {
                    Debug.LogError($"No Generator of type {settings.GetType()}");
                }
            }

            for (int i = 0; i < evaluationContexts.Length; i++)
            {
                evaluationContexts[i].SetSamplePoints(data);
            }
        }

        public void GenerateDebugPoints(List<SpatialQuerySamplePoint> points)
        {
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
                availablePoints[i].transform.position = points[i].PointPosition;
                availablePoints[i].SetValue(points[i]);
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
