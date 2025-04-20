using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SpatialQuery
{
    public abstract class SpatialQueryGenerator : ScriptableObject
    {
        protected Transform _querier;
        protected Transform _target;
        protected bool _isDebugMode = false;
        protected QueryDebugger _debugger;

        public void EnableDebugMode(QueryDebugger debugger)
        {
            _isDebugMode = true;
            _debugger = debugger;
        }

        protected SpatialQuerySamplePoint RequestPoint()
        {
            if (_isDebugMode)
            {
                return _debugger.RequestSamplePoint();
            }
            else
            {
                return SpatialQuerySystem.GetInstance().RequestSamplePoint();
            }
        }

        public virtual List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            _querier = context.Querier;
            _target = context.Target;
            
            return null;
        }

        protected bool IsPointValid(Vector3 sourcePoint, Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(sourcePoint, point, NavMesh.AllAreas, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public abstract Vector2 GetAreaCoverageRange();
    }
}
