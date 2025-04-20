using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [CustomEditor(typeof(SpatialQueryEvaluator), true)]
    public class SpatialQueryEvaluatorEditor : Editor
    {
        private bool _showSummaryFoldout = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpatialQueryEvaluator evaluator = (SpatialQueryEvaluator)target;


            EditorGUI.indentLevel++;

            _showSummaryFoldout = EditorGUILayout.Foldout(_showSummaryFoldout, "Note", true);

            if (_showSummaryFoldout)
            {
                EditorGUI.indentLevel++;
                evaluator.Note = EditorGUILayout.TextArea(evaluator.Note, GUILayout.MinHeight(60), GUILayout.ExpandHeight(true));
                //evaluator.Note = EditorGUILayout.TextArea(evaluator.Note, GUILayout.MinHeight(60));
                EditorGUI.indentLevel--;
            }

            ScoringMode selectedMode = evaluator.ScoringMode;
            ScoringMode newMode = (ScoringMode)EditorGUILayout.EnumPopup("Scoring Mode", selectedMode);
            evaluator.SetScoringMode(newMode);

            if (newMode != ScoringMode.Score)
            {
                Vector2 vec = evaluator.FilteringScore;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Filter out Score", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.MinMaxSlider(ref vec.x, ref vec.y, 0f, 1f, GUILayout.Width(150));
                GUILayout.Space(10);
                vec.x = EditorGUILayout.FloatField(vec.x, GUILayout.Width(60));
                vec.y = EditorGUILayout.FloatField(vec.y, GUILayout.Width(60));
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();

                vec.x = Mathf.Clamp01(vec.x);
                vec.y = Mathf.Clamp01(vec.y);
                evaluator.FilteringScore = vec; // <- this was missing
            }

            if (newMode != ScoringMode.Filter)
            {
                evaluator.SetScoreWeight(EditorGUILayout.FloatField("Score Weight", evaluator.ScoreWeight));
            }

            DrawPropertiesExcluding(serializedObject,
                nameof(SpatialQueryEvaluator.ScoringMode),
                nameof(SpatialQueryEvaluator.FilteringScore),
                "m_Script");

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10f);
        }

    }
}
