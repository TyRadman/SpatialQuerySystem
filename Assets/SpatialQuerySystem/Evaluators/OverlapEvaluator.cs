using System.Text;
using UnityEngine;

namespace SpatialQuery
{
    public class OverlapEvaluator : SpatialQueryEvaluator
    {
        [SerializeField] private LayerMask _overlapLayer;
        [SerializeField, Min(0f)] private float _sphereRadius = 1f;
        [SerializeField] private Vector3 _positionOffset = Vector3.zero;
        public bool OverlapIsHighScore = true;

        public override string GetIconPath()
        {
            return "T_OverlapIcon.png";
        }

        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            Vector3 position = samplePoint.PointPosition + _positionOffset;
            bool hasOverlap = Physics.CheckSphere(position, _sphereRadius, _overlapLayer);
            float score = hasOverlap == OverlapIsHighScore ? 1f : 0f;
            AddToScore(samplePoint, score);
        }

        public override string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.Append(base.GetEvaluatorSummary());

            summary.Append($"<b>Overlap sphere radius</b>: {_sphereRadius}");

            summary.Append($"\n");

            return summary.ToString();
        }

        public override void SetRange(Vector2 range)
        {
            // do nothing
        }
    }
}
