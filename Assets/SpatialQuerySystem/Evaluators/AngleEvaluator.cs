using System.Text;
using UnityEngine;

namespace SpatialQuery
{
    public class AngleEvaluator : SpatialQueryEvaluator
    {
        public float MinAngle = 30f;
        public float MaxAngle = 90f;
        public bool CloseToSightIsHighScore = true;

        private float _minAngle = 0f;
        private float _maxAngle = 0f;

        protected override void Awake()
        {
            base.Awake();

            _minAngle = MinAngle / 2f;
            _maxAngle = MaxAngle / 2f;
        }

        public override string GetIconPath()
        {
            return "T_AngleIcon.png";
        }

        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            Vector3 forward = (context.Target.position - context.Querier.position);
            Vector3 origin = context.Querier.position;

            Vector3 toPoint = (samplePoint.PointPosition - origin).normalized;

            float angle = Vector3.Angle(forward, toPoint);
            float scoreToAdd = 0f;

            if (angle < _minAngle)
            {
                scoreToAdd = 0f;
            }
            else if (angle <= _maxAngle)
            {
                scoreToAdd = Mathf.InverseLerp(_minAngle, _maxAngle, angle);
            }
            else if (angle > _maxAngle)
            {
                scoreToAdd = 1f;
            }

            if(CloseToSightIsHighScore)
            {
                scoreToAdd = 1 - scoreToAdd;
            }

            AddToScore(samplePoint, scoreToAdd);
        }

        public override string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.Append(base.GetEvaluatorSummary());

            summary.Append($"<b>Angle detection range</b>: {MinAngle} - {MaxAngle} degrees");

            summary.Append($"\n");

            return summary.ToString();
        }
    }
}
