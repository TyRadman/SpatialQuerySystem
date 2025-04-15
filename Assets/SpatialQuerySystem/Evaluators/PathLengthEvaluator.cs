using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace SpatialQuery
{
    public class PathLengthEvaluator : SpatialQueryEvaluator
    {
        public float MinPathLength = 2f;
        public float MaxPathLength = 10f;
        public bool CloserIsHigherScore = false;

        public override string GetIconPath()
        {
            return "T_PathIcon.png";
        }

        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            Vector3 targetPosition = context.Target.position;
            float maxDistance = MaxPathLength;
            float minDistance = MinPathLength;

            Vector3 startPoint = samplePoint.PointPosition;
            NavMeshPathStatus status = NavMeshPathStatus.PathInvalid;
            float pathLength = GetPathLength(startPoint, targetPosition, ref status);

            float scoreToAdd;

            if (status == NavMeshPathStatus.PathInvalid)
            {
                scoreToAdd = 0f;
            }
            else if (CloserIsHigherScore)
            {
                scoreToAdd = status != NavMeshPathStatus.PathPartial ? Mathf.InverseLerp(maxDistance, minDistance, pathLength) : 0f;
            }
            else
            {
                scoreToAdd = status != NavMeshPathStatus.PathPartial ? Mathf.InverseLerp(minDistance, maxDistance, pathLength) : 1f;
            }

            AddToScore(samplePoint, scoreToAdd);
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

        public override string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.Append(base.GetEvaluatorSummary());

            summary.Append($"<b>Path length range</b>: {MinPathLength} - {MaxPathLength}");

            summary.Append($"\n");

            return summary.ToString();
        }
    }
}
