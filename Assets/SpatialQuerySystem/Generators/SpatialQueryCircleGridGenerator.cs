using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialQueryCircleGridGenerator : SpatialQueryGenerator
    {
        [Header("Size")]
        public float InnerRadius = 5f;
        public float OuterRadius = 20f;

        [Header("Sample points")]
        public float Spacing = 2f;

        [Header("Others")]
        public float OffsetY = 1.1f;

        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(context);

            float outerRadius = OuterRadius;
            float innerRadius = InnerRadius;
            float spacing = Spacing;
            Vector2 centerPoint = new Vector2(context.GetSamplingCenter().x, context.GetSamplingCenter().z);

            int pointsPerSide = Mathf.Max(1, Mathf.FloorToInt((outerRadius * 2f) / spacing) + 1);
            float totalGridSize = (pointsPerSide - 1) * spacing;
            Vector2 startPoint = centerPoint - new Vector2(totalGridSize / 2f, totalGridSize / 2f);

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            for (int i = 0; i < pointsPerSide; i++)
            {
                for (int j = 0; j < pointsPerSide; j++)
                {
                    Vector2 point = startPoint + new Vector2(i * spacing, j * spacing);

                    float squaredDistance = (point - centerPoint).sqrMagnitude;

                    if (squaredDistance > innerRadius * innerRadius && squaredDistance < outerRadius * outerRadius)
                    {
                        Vector3 position = new Vector3(point.x, OffsetY, point.y);

                        if (!IsPointValid(_querier.position, position))
                        {
                            continue;
                        }

                        SpatialQuerySamplePoint samplePoint = RequestPoint();
                        samplePoint.PointPosition = position;
                        points.Add(samplePoint);
                    }
                }
            }

            return points;
        }

        public override Vector2 GetAreaCoverageRange()
        {
            return new Vector2(InnerRadius, OuterRadius);
        }
    }
}
