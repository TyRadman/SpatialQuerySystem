using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class RingGenerator : SpatialQueryGenerator
    {
        [Header("Size")]
        public float InnerRadius = 5f;
        public float OuterRadius = 20f;
        [Header("Sample points")]
        public int RingCount = 8;
        public int PointsPerRing = 24;
        [Header("Others")]
        public float OffsetY = 1.1f;

        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(context);
            
            Vector3 center = context.GetSamplingCenter();
            center.y = OffsetY;

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            float radialStep = (OuterRadius - InnerRadius) /Mathf.Max(1, RingCount - 1);

            for (int ring = 0; ring < RingCount; ring++)
            {
                float currentRadius = InnerRadius + (radialStep * ring);
                CreateRingPoints(currentRadius, PointsPerRing, center, points);
            }

            return points;
        }

        private void CreateRingPoints(float radius, int pointCount, Vector3 center,List<SpatialQuerySamplePoint> points)
        {
            float angleStep = 360f / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * angleStep;
                Vector3 position = CalculateRingPosition(center, radius, angle);

                if (!IsPointValid(position, _target.position))
                {
                    continue;
                }

                SpatialQuerySamplePoint point = RequestPoint();
                point.PointPosition = position;
                points.Add(point);
            }
        }

        private Vector3 CalculateRingPosition(Vector3 center, float radius, float angle)
        {
            return center + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        }

        public override Vector2 GetAreaCoverageRange()
        {
            return new Vector2(InnerRadius, OuterRadius);
        }
    }
}
