using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class RingGenerator : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(GeneratorSettings generatorSettings, SpatialSamplingContext context)
        {
            base.GenerateSamplePoints(generatorSettings, context);

            if (!ValidateSettings<RingGeneratorSettings>(generatorSettings, out var settings))
            {
                return null;
            }
            
            Vector3 center = context.GetSamplingCenter();
            center.y = settings.OffsetY;

            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            float radialStep = (settings.OuterRadius - settings.InnerRadius) /Mathf.Max(1, settings.RingCount - 1);

            for (int ring = 0; ring < settings.RingCount; ring++)
            {
                float currentRadius = settings.InnerRadius + (radialStep * ring);
                CreateRingPoints(currentRadius, settings.PointsPerRing, center, points);
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

                SpatialQuerySamplePoint point = SpatialQuerySystem.GetInstance().RequestSamplePoint();
                point.PointPosition = position;
                points.Add(point);
            }
        }

        private Vector3 CalculateRingPosition(Vector3 center, float radius, float angle)
        {
            return center + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        }
    }
}
