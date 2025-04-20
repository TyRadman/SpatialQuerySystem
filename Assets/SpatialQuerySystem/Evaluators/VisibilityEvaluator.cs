using System.Text;
using UnityEngine;

namespace SpatialQuery
{
    public class VisibilityEvaluator : SpatialQueryEvaluator
    {
        public LayerMask BlockerLayer;
        public float VisibilityDistance;
        [SerializeField, Min(0f)] private float _visibilityRadius = 0f;
        public bool VisibilityIsHighScore = true;
        public float OffsetY = 1.1f;

        public override string GetIconPath()
        {
            return "T_VisibilityIcon.png";
        }

        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            Transform target = context.Target;

            Vector3 targetPosition = target.position;
            targetPosition.y += OffsetY;

            Vector3 pointPosition = samplePoint.PointPosition;
            pointPosition.y += OffsetY;

            Ray ray = new Ray(pointPosition, targetPosition - pointPosition);
            float distance = Vector3.Distance(pointPosition, targetPosition);
            float scoreToAdd;

            if (_visibilityRadius > 0f)
            {
                if (Physics.SphereCast(ray, _visibilityRadius, out RaycastHit hit, distance, BlockerLayer))
                {
                    bool targetHit = hit.transform == target;
                    scoreToAdd = (targetHit == VisibilityIsHighScore) ? 1f : 0f;
                }
                else
                {
                    scoreToAdd = VisibilityIsHighScore ? 0f : 1f;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit hit, distance, BlockerLayer))
                {
                    bool targetHit = hit.transform == target;
                    scoreToAdd = (targetHit == VisibilityIsHighScore) ? 1f : 0f;
                }
                else
                {
                    scoreToAdd = VisibilityIsHighScore ? 0f : 1f;
                }
            }


            AddToScore(samplePoint, scoreToAdd);
        }

        public override string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.Append(base.GetEvaluatorSummary());

            summary.Append($"<b>Visibility distance</b>: {VisibilityDistance}");
            summary.Append($"\n<b>Visibility radius</b>: {_visibilityRadius}");

            summary.Append($"\n");

            return summary.ToString();
        }

        public override void SetRange(Vector2 range)
        {
            VisibilityDistance = range.y;
        }
    }
}
