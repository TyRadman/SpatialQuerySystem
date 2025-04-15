using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialQueryCircleGridGenerator : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(GeneratorSettings generatorSettings, SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(generatorSettings, context);

            if (generatorSettings == null || generatorSettings is not CircleGridGeneratorSettings settings)
            {
                Debug.LogError("Generator settings are either null or the wrong type");
                return null;
            }

            float outerRadius = settings.OuterRadius;
            float innerRadius = settings.InnerRadius;
            float spacing = settings.Spacing;
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
                        Vector3 position = new Vector3(point.x, settings.OffsetY, point.y);

                        if (!IsPointValid(_querier.position, position))
                        {
                            continue;
                        }

                        SpatialQuerySamplePoint samplePoint = SpatialQuerySystem.GetInstance().RequestSamplePoint();
                        samplePoint.PointPosition = position;
                        points.Add(samplePoint);
                    }
                }
            }

            return points;
        }
    }
}
