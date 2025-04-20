using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SpatialQuery
{
    public abstract class SpatialQueryEvaluator : ScriptableObject
    {
        public Vector2 FilteringScore = new Vector2(0.5f, 1f);
        public ScoringMode ScoringMode { get; private set; } = ScoringMode.Score;
        public float ScoreWeight { get; private set; } = 1f;

#if UNITY_EDITOR
        [HideInInspector] public string Note = "Write your note here";
#endif

        protected virtual void Awake()
        {

        }

        public virtual void Init(Transform owner)
        {

        }

        protected abstract void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint);

        public virtual void Evaluate(SpatialEvaluationContext context)
        {
            if (context == null)
            {
                Debug.LogError("No context passed");
                return;
            }

            if(context.Target == null)
            {
                Debug.LogError("No target");
                return;
            }

            List<SpatialQuerySamplePoint> points = context.GetSampledPoints();

            for (int i = 0; i < points.Count; ++i)
            {
                SpatialQuerySamplePoint point = points[i];

                if (point == null)
                {
                    Debug.LogError("Point is null");
                    continue;
                }

                SetSamplePointScore(context, point);

                if(point.Score > context.HighestScore && !point.IsFilteredOut)
                {
                    context.HighestScore = point.Score;
                    context.HighestScoreIndex = i;
                }
            }
        }

        protected void AddToScore(SpatialQuerySamplePoint data, float scoreToAdd)
        {
            RegisterReport(data, scoreToAdd);

            if(ScoringMode != ScoringMode.Score)
            {
                if (scoreToAdd >= FilteringScore.x && scoreToAdd <= FilteringScore.y)
                {
                    data.IsFilteredOut = true;

                    if (ScoringMode == ScoringMode.Filter)
                    {
                        return;
                    }
                }
            }

            data.Score += scoreToAdd * ScoreWeight;
        }

        private void RegisterReport(SpatialQuerySamplePoint data, float scoreToAdd)
        {
            const int colWidth = 40;
            const int numColumnWidth = 10;

            if (data.Report.Length == 0)
            {
                string header = $"<b>{"Evaluator",-colWidth}</b>" +
                                $"<b>{"Score",numColumnWidth}</b>" +
                                $"<b>{"Weight",numColumnWidth}</b>" +
                                $"<b>{"Total",numColumnWidth}</b>";
                data.Report.AppendLine(header);
            }

            string name = GetType().Name;
            string paddedName = name.PadRight(colWidth);

            data.Report.AppendLine(
                $"{paddedName}" +
                $"{scoreToAdd,numColumnWidth:0.0}" +
                $"{ScoreWeight,numColumnWidth:0.0}" +
                $"{(scoreToAdd * ScoreWeight),numColumnWidth:0.0}"
            );

        }

        public void SetScoringMode(ScoringMode mode)
        {
            ScoringMode = mode;
        }

        protected Vector2 GetSubjectPosition(SpatialQuerySubjectTarget subjectType, SpatialEvaluationContext context, SpatialQueryEvaluatorSubjectBase customSubject = null)
        {
            Vector3 position = Vector3.zero;

            switch (subjectType)
            {
                case SpatialQuerySubjectTarget.Querier:
                    position = context.Querier.position;
                    break;
                case SpatialQuerySubjectTarget.Target:
                    position = context.Target.position;
                    break;
                case SpatialQuerySubjectTarget.Custom:
                    position = customSubject.GetEvaluatorSubject().position;
                    break;
            }

            return position.ToVector2();
        }

        public void SetScoreWeight(float scoreWeight)
        {
            ScoreWeight = scoreWeight;
        }

        public virtual string GetIconPath()
        {
            return "T_FolderIcon.png";
        }

        public virtual string GetEvaluatorSummary()
        {
            StringBuilder summary = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Note))
            {
                summary.Append($"\n<b>Note</b>: {Note}");
                summary.Append($"\n");
            }

            summary.Append($"\n");

            summary.Append($"<b>Scoring mode</b>: {ScoringMode}. ");

            if (ScoringMode != ScoringMode.Score)
            {
                summary.Append($"<b>Filter out score</b>: {FilteringScore.x:0.0} - {FilteringScore.y:0.0}");
            }

            summary.Append($"\n<b>Score weight</b>: {ScoreWeight:0.0}");
            summary.Append($"\n\n");

            return summary.ToString();
        }

        public abstract void SetRange(Vector2 range);
    }
}
