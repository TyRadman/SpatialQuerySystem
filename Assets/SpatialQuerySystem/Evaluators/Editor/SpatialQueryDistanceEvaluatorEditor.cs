using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [CustomEditor(typeof(DistanceEvaluator))]
    public class SpatialQueryDistanceEvaluatorEditor : SpatialQueryEvaluatorEditor
    {
        private Dictionary<string, System.Type> _subjectTypes;
        private List<string> _subjectTypeNames = new List<string>();
        private int _selectedSubjectIndex = -1;

        private bool _initialized = false;

        SerializedProperty subjectTypeProp;
        SerializedProperty closerHighEvalProp;
        SerializedProperty minDistanceProp;
        SerializedProperty maxDistanceProp;
        SerializedProperty customSubjectTypeNameProp;

        private void InitSubjectTypes()
        {
            if (_initialized)
            {
                return;
            }

            _subjectTypes = new Dictionary<string, System.Type>();
            _subjectTypeNames = new List<string>();

            foreach (System.Type type in System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(SpatialQueryEvaluatorSubjectBase)) && !t.IsAbstract))
            {
                string name = ObjectNames.NicifyVariableName(type.Name);
                _subjectTypes.Add(name, type);
                _subjectTypeNames.Add(name);
            }

            _initialized = true;
        }

        private void OnEnable()
        {
            InitSubjectTypes();

            subjectTypeProp = serializedObject.FindProperty(nameof(DistanceEvaluator.SubjectType));
            closerHighEvalProp = serializedObject.FindProperty(nameof(DistanceEvaluator.CloserIsHighEvaluation));
            minDistanceProp = serializedObject.FindProperty(nameof(DistanceEvaluator.MinDistance));
            maxDistanceProp = serializedObject.FindProperty(nameof(DistanceEvaluator.MaxDistance));
            customSubjectTypeNameProp = serializedObject.FindProperty(nameof(DistanceEvaluator.CustomSubjectTypeName));
        }

        public override void OnInspectorGUI()
        {
            DistanceEvaluator evaluator = (DistanceEvaluator)target;

            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(subjectTypeProp);

            if ((SpatialQuerySubjectTarget)subjectTypeProp.enumValueIndex == SpatialQuerySubjectTarget.Custom)
            {
                _selectedSubjectIndex = Mathf.Max(0, _subjectTypeNames.FindIndex(name => _subjectTypes[name].AssemblyQualifiedName == customSubjectTypeNameProp.stringValue));

                int newIndex = EditorGUILayout.Popup("Custom Subject Type", _selectedSubjectIndex, _subjectTypeNames.ToArray());
                _selectedSubjectIndex = newIndex;

                if (customSubjectTypeNameProp.stringValue != _subjectTypeNames[newIndex])
                {
                    customSubjectTypeNameProp.stringValue = _subjectTypes[_subjectTypeNames[_selectedSubjectIndex]].AssemblyQualifiedName;
                }
            }

            GUILayout.Space(20f);

            //EditorGUILayout.PropertyField(customSubjectTypeNameProp);

            EditorGUILayout.PropertyField(closerHighEvalProp);
            EditorGUILayout.PropertyField(minDistanceProp);
            EditorGUILayout.PropertyField(maxDistanceProp);


            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10f);
        }
    }
}
