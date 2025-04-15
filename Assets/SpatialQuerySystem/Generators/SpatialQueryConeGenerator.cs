using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialQueryConeGenerator : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(GeneratorSettings generatorSettings, SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(generatorSettings, context);

            if (!ValidateSettings<ConeGeneratorSettings>(generatorSettings, out var settings))
            {
                return null;
            }

            Vector3 center = context.GetSamplingCenter();
            center.y = settings.OffsetY;

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            Vector3 forward = context.Target.position - context.Querier.position;
            forward.y = 0;
            forward.Normalize();

            float forwardAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            float halfAngle = settings.Angle * 0.5f;
            float startAngle = forwardAngle - halfAngle;
            float endAngle = forwardAngle + halfAngle;

            float radiusStep = (settings.OuterRadius - settings.InnerRadius) / Mathf.Max(1, settings.RingCount - 1);

            for (int layer = 0; layer < settings.RingCount; layer++)
            {
                float currentRadius = settings.InnerRadius + radiusStep * layer;
                CreateLayerPoints(currentRadius, settings.PointsPerRing, center, startAngle, endAngle, points);
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

                SpatialQuerySamplePoint point = SpatialQuerySystem.GetInstance().RequestSamplePoint();
                point.PointPosition = position;
                points.Add(point);
            }
        }

        private Vector3 CalculatePosition(Vector3 center, float radius, float angle)
        {
            return center + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        }
    }
}
