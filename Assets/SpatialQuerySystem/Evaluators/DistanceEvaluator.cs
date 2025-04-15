using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SpatialQuery
{
    public class DistanceEvaluator : SpatialQueryEvaluator
    {
        public System.Type CustomSubjectType;

        [HideInInspector] public SpatialQuerySubjectTarget SubjectType = SpatialQuerySubjectTarget.Querier; 
        [HideInInspector] public string CustomSubjectTypeName;
        [HideInInspector] public bool CloserIsHighEvaluation = false;
        [HideInInspector] public float MinDistance = 1f;
        [HideInInspector] public float MaxDistance = 5f;
        
        protected SpatialQueryEvaluatorSubjectBase _customSubject;

        public override string GetIconPath()
        {
            return "T_DistanceIcon.png";
        }

        public override void Init(Transform owner)
        {
            base.Init(owner);

            if (SubjectType != SpatialQuerySubjectTarget.Custom)
            {
                return;
            }

            if (CustomSubjectType == null)
            {
                CustomSubjectType = string.IsNullOrEmpty(CustomSubjectTypeName) ? null : System.Type.GetType(CustomSubjectTypeName);
            }

            _customSubject = System.Activator.CreateInstance(CustomSubjectType) as SpatialQueryEvaluatorSubjectBase;

            if (_customSubject == null)
            {
                Debug.LogError("Failed to create");
            }
            else
            {
                _customSubject.SetUp(owner.gameObject);
            }
        }

        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            Vector2 targetPosition = GetSubjectPosition(SubjectType, context, _customSubject);

            float distance = Vector2.Distance(samplePoint.PointPosition.ToVector2(), targetPosition);
            float scoreToAdd = 0f;

            if (distance <= MaxDistance && distance >= MinDistance)
            {
                if (!CloserIsHighEvaluation)
                {
                    scoreToAdd = Mathf.InverseLerp(MinDistance, MaxDistance, distance);
                }
                else
                {
                    scoreToAdd = Mathf.InverseLerp(MaxDistance, MinDistance, distance);
                }

                // use running average equation to skip caching stuff
            }
            else if (distance < MinDistance && CloserIsHighEvaluation)
            {
                scoreToAdd = 1f;
            }
            else if (distance > MaxDistance && !CloserIsHighEvaluation)
            {
                scoreToAdd = 1f;
            }

            //$"Distance from {targetPosition} to {samplePoint.PointPosition} scored {scoreToAdd}".Log();

            AddToScore(samplePoint, scoreToAdd);
        }

        public override string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.Append(base.GetEvaluatorSummary());

            summary.Append($"<b>Distance range</b>: {MinDistance} - {MaxDistance}");

            summary.Append($"\n");

            return summary.ToString();
        }
    }
}
