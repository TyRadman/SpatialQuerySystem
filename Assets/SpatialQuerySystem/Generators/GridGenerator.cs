using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class GridGenerator : SpatialQueryGenerator
    {
        [Header("Size")]
        public float CoverageSize = 20f;

        [Header("Sample points")]
        public float Spacing = 2f;

        [Header("Others")]
        public float OffsetY = 1.1f;

        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(context);

            float size = CoverageSize;
            float spacing = Spacing;
            int pointsPerSide = Mathf.Max(1, Mathf.FloorToInt(size / spacing) + 1);
            float totalGridSize = (pointsPerSide - 1) * spacing;
            Vector2 centerPoint = new Vector2(context.GetSamplingCenter().x, context.GetSamplingCenter().z);
            Vector2 startPoint = centerPoint - new Vector2(totalGridSize / 2f, totalGridSize / 2f);

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            for (int i = 0; i < pointsPerSide; i++)
            {
                for (int j = 0; j < pointsPerSide; j++)
                {
                    Vector2 point = startPoint + new Vector2(i * spacing, j * spacing);
                    Vector3 position = new Vector3(point.x, OffsetY, point.y);

                    if(!IsPointValid(_querier.position, position))
                    {
                        continue;
                    }

                    SpatialQuerySamplePoint samplePoint = RequestPoint();
                    samplePoint.PointPosition = position;
                    points.Add(samplePoint);
                }
            }

            return points;
        }

        public override Vector2 GetAreaCoverageRange()
        {
            return new Vector2(0f, CoverageSize);
        }
    }
}
