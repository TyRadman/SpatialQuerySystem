using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class GridGenerator : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(GeneratorSettings generatorSettings, SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(generatorSettings, context);

            if (generatorSettings == null || generatorSettings is not GridGeneratorSettings settings)
            {
                Debug.LogError("Generator settings are either null or the wrong type");
                return null;
            }

            float size = settings.CoverageSize;
            float spacing = settings.Spacing;
            int pointsPerSide = Mathf.Max(1, Mathf.FloorToInt(size / spacing) + 1);
            float totalGridSize = (pointsPerSide - 1) * spacing;
            Vector3 startPoint = context.GetSamplingCenter() - new Vector3(totalGridSize / 2f, 0f, totalGridSize / 2f);

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            for (int i = 0; i < pointsPerSide; i++)
            {
                for (int j = 0; j < pointsPerSide; j++)
                {
                    Vector3 position = startPoint + new Vector3(i * spacing, settings.OffsetY, j * spacing);

                    if(!IsPointValid(_querier.position, position))
                    {
                        continue;
                    }

                    SpatialQuerySamplePoint samplePoint = SpatialQuerySystem.GetInstance().RequestSamplePoint();
                    samplePoint.PointPosition = position;
                    points.Add(samplePoint);
                }
            }

            return points;
        }
    }
}
