using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialSamplingContext
    {
        public Transform Querier { get; private set; }
        public Transform Target { get; private set; }

        private Vector3 _samplingCenter;
        private List<GeneratorSettings> _generators = new List<GeneratorSettings>();

        public SpatialSamplingContext(params GeneratorSettings[] generatorSettings)
        {
            for (int i = 0; i < generatorSettings.Length; i++)
            {
                _generators.Add(generatorSettings[i]);
            }
        }

        public List<GeneratorSettings> GetGenerators()
        {
            return _generators;
        }

        public Vector3 GetSamplingCenter()
        {
            return _samplingCenter;
        }

        public void SetSamplingPoint(Vector3 samplingPoint)
        {
            _samplingCenter = samplingPoint;
        }

        public void SetTarget(Transform target)
        {
            if (target == null)
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
