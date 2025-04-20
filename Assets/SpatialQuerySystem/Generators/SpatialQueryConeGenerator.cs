using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialQueryConeGenerator : SpatialQueryGenerator
    {
        [Header("Size")]
        public float InnerRadius = 5f;
        public float OuterRadius = 20f;

        [Header("Sample points")]
        public int RingCount = 7;
        public int PointsPerRing = 24;

        [Header("Others")]
        public float Angle = 180f;
        public float OffsetY = 1.1f;

        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(context);

            Vector3 center = context.GetSamplingCenter();
            center.y = OffsetY;

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            Vector3 forward = context.Target.position - context.Querier.position;
            forward.y = 0;
            forward.Normalize();

            float forwardAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            float halfAngle = Angle * 0.5f;
            float startAngle = forwardAngle - halfAngle;
            float endAngle = forwardAngle + halfAngle;

            float radiusStep = (OuterRadius - InnerRadius) / Mathf.Max(1, RingCount - 1);

            for (int layer = 0; layer < RingCount; layer++)
            {
                float currentRadius = InnerRadius + radiusStep * layer;
                CreateLayerPoints(currentRadius, PointsPerRing, center, startAngle, endAngle, points);
            }

            return points;
        }

        private void CreateLayerPoints(float radius, int pointsPerLayer, Vector3 center, float startAngle, float endAngle, List<SpatialQuerySamplePoint> points)
        {
            if (pointsPerLayer <= 0)
            {
                return;
            }

            float angleStep = (endAngle - startAngle) / Mathf.Max(1, pointsPerLayer - 1);

            for (int i = 0; i < pointsPerLayer; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector3 position = CalculatePosition(center, radius, angle);

                if (!IsPointValid(position, _target.position))
                {
                    continue;
                }

                SpatialQuerySamplePoint point = RequestPoint();
                point.PointPosition = position;
                points.Add(point);
            }
        }

        private Vector3 CalculatePosition(Vector3 center, float radius, float angle)
        {
            return center + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        }

        public override Vector2 GetAreaCoverageRange()
        {
            return new Vector2(InnerRadius, OuterRadius);
        }
    }
}
