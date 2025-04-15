using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SpatialQuery
{
    public abstract class SpatialQueryGenerator
    {
        protected Transform _querier;
        protected Transform _target;
        public virtual List<SpatialQuerySamplePoint> GenerateSamplePoints(GeneratorSettings generatorSettings, SpatialSamplingContext context)
        {
            _querier = context.Querier;
            _target = context.Target;
            
            return null;
        }

        protected bool IsPointValid(Vector3 sourcePoint, Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(sourcePoint, point, NavMesh.AllAreas, path);
            return path.status != NavMeshPathStatus.PathInvalid;
        }

        protected float GetPathLength(Vector3 startPoint, Vector3 destination, ref NavMeshPathStatus pathStatus)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(startPoint, destination, NavMesh.AllAreas, path);

            float length = 0f;

            if (path.corners.Length < 2)
            {
                return length;
            }

            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            pathStatus = path.status;

            return length;
        }

        protected bool ValidateSettings<T>(GeneratorSettings settings, out T validated) where T : GeneratorSettings
        {
            validated = null;

            if (settings == null || !(settings is T))
            {
                Debug.LogError("Invalid generator settings");
                return false;
            }

            validated = (T)settings;
            return true;
        }
    }
}
