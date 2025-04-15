using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpatialQuery
{
    [CustomEditor(typeof(SpatialQueryAsset))]
    public class SpatialQueryAssetEditor : Editor
    {
        private Dictionary<Type, Texture2D> _iconCache = new();
        private Dictionary<Object, Editor> _cachedEditors = new();
        private List<FieldInfo> _settingsFields = new();
        private List<Type> settingTypes;
        private string[] _fieldNames;
        private string[] typeNames;
        private Type baseType = typeof(SpatialQueryEvaluator);
        private Texture2D _arrowDownIcon;
        private Texture2D _arrowUpIcon;
        private int newEvalIndex = 0;
        private int _activeEvaluatorIndex = 0;

        private const float EVALUATOR_LABEL_HEIGHT = 26;

        private void OnEnable()
        {
            CacheGenerators();

            CacheEvaluatorTypes();

            _arrowUpIcon = SpatialQueryEditorAssets.LoadIcon("T_Arrow_Up.png");
            _arrowDownIcon = SpatialQueryEditorAssets.LoadIcon("T_Arrow_Down.png");
        }

        private void CacheGenerators()
        {
            _settingsFields.Clear();

            var containerType = typeof(SpatialQueryAsset);

            foreach (FieldInfo field in containerType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType.IsAbstract)
                {
                    continue;
                }

                if (typeof(GeneratorSettings).IsAssignableFrom(field.FieldType))
                {
                    _settingsFields.Add(field);
                }
            }

            _fieldNames = _settingsFields.ConvertAll(f => ObjectNames.NicifyVariableName(f.Name)).ToArray();
        }

        private void CacheEvaluatorTypes()
        {
            baseType = typeof(SpatialQueryEvaluator);
            settingTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType)
                .ToList();

            typeNames = settingTypes.Select(t => ObjectNames.NicifyVariableName(t.Name)).ToArray();
        }


        public override void OnInspectorGUI()
        {
            var container = (SpatialQueryAsset)target;
            var so = new SerializedObject(container);
            so.Update();

            container.DebugPoints = EditorGUILayout.Toggle("Debug sample points", container.DebugPoints);
            container.GetHighestScore = EditorGUILayout.Toggle("Select highest score", container.GetHighestScore);

            if (!container.GetHighestScore)
            {
                Vector2 vec = container.SuccessScoreRange;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Success Range", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.MinMaxSlider(ref vec.x, ref vec.y, 0f, 1f);
                GUILayout.Space(10);
                vec.x = EditorGUILayout.FloatField(vec.x, GUILayout.Width(50));
                vec.y = EditorGUILayout.FloatField(vec.y, GUILayout.Width(50));
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();

                vec.x = Mathf.Clamp01(vec.x);
                vec.y = Mathf.Clamp01(vec.y);
                container.SuccessScoreRange = vec;
            }

            GUILayout.Space(10f);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // generator dropdown
            container.SelectedIndex = EditorGUILayout.Popup("Generator Type", container.SelectedIndex, _fieldNames);

            if (container.SelectedIndex >= 0 && container.SelectedIndex < _settingsFields.Count)
            {
                var selectedField = _settingsFields[container.SelectedIndex];
                var selectedSettings = (GeneratorSettings)selectedField.GetValue(container);
                container.SetSelectedGenerator(selectedSettings);

                SerializedProperty selectedProp = so.FindProperty("_selectedGenerator");

                if (selectedProp != null)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(selectedProp, true);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // evaluator type dropdown and the add button
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle bigLabel = new GUIStyle(EditorStyles.boldLabel);
            bigLabel.fontSize = 16;
            EditorGUILayout.LabelField("Evaluators", bigLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            newEvalIndex = EditorGUILayout.Popup(newEvalIndex, typeNames);

            if (GUILayout.Button("Add Evaluator", GUILayout.Width(120)))
            {
                var newType = settingTypes[newEvalIndex];
                var newEvaluator = ScriptableObject.CreateInstance(newType) as SpatialQueryEvaluator;

                string path = AssetDatabase.GetAssetPath(container);
                if (!string.IsNullOrEmpty(path))
                {
                    newEvaluator.name = newType.Name;
                    AssetDatabase.AddObjectToAsset(newEvaluator, path);
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.SaveAssets();

                    container.AddEvaluator(newEvaluator);
                    EditorUtility.SetDirty(container);
                }
                else
                {
                    Debug.LogError("Container must be an asset to hold sub-assets.");
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);

            RenderEvaluators(container);

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            so.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(container);
            }
        }

        private void RenderEvaluators(SpatialQueryAsset container)
        { 
            var evaluators = container.GetEvaluators();

            for (int i = 0; i < evaluators.Count; i++)
            {
                SpatialQueryEvaluator evaluator = evaluators[i];
                
                if (evaluator == null)
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();

                GUI.enabled = i > 0;

                if (GUILayout.Button(new GUIContent(_arrowUpIcon, "Move evaluator to higher priority"), GUILayout.Width(EVALUATOR_LABEL_HEIGHT), GUILayout.Height(EVALUATOR_LABEL_HEIGHT)))
                {
                    container.SwapEvaluators(i, i - 1);

                    if (_activeEvaluatorIndex == i)
                    {
                        _activeEvaluatorIndex = i - 1;
                    }
                    else if (_activeEvaluatorIndex == i - 1)
                    {
                        _activeEvaluatorIndex = i;
                    }

                    EditorUtility.SetDirty(container);
                    GUIUtility.ExitGUI();
                }

                GUI.enabled = i < evaluators.Count - 1;

                if (GUILayout.Button(new GUIContent(_arrowDownIcon, "Move evaluator to lower priority"), GUILayout.Width(EVALUATOR_LABEL_HEIGHT), GUILayout.Height(EVALUATOR_LABEL_HEIGHT)))
                {
                    container.SwapEvaluators(i, i + 1);

                    if (_activeEvaluatorIndex == i)
                    {
                        _activeEvaluatorIndex = i + 1;
                    }
                    else if (_activeEvaluatorIndex == i + 1)
                    {
                        _activeEvaluatorIndex = i;
                    }

                    EditorUtility.SetDirty(container);
                    GUIUtility.ExitGUI();
                }

                GUI.enabled = true;

                Texture2D icon = null;
                Type evaluatorType = evaluator.GetType();

                if (!_iconCache.TryGetValue(evaluatorType, out icon))
                {
                    string path = evaluator.GetIconPath();

                    if (!string.IsNullOrEmpty(path))
                    {
                        icon = SpatialQueryEditorAssets.LoadIcon(path);// AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    }

                    _iconCache[evaluatorType] = icon; 
                }

                if (icon != null)
                {
                    GUILayout.Label(icon, GUILayout.Width(EVALUATOR_LABEL_HEIGHT), GUILayout.Height(EVALUATOR_LABEL_HEIGHT));
                }
                else
                {
                    // empty space if there's no icon
                    GUILayout.Space(EVALUATOR_LABEL_HEIGHT + 2);
                }

                GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16
                };

                //GUILayout.Label(ObjectNames.NicifyVariableName(evaluator.GetType().Name), nameStyle, GUILayout.ExpandWidth(true));

                GUILayout.BeginVertical();

                GUILayout.Label(ObjectNames.NicifyVariableName(evaluator.GetType().Name), nameStyle, GUILayout.ExpandWidth(true));

                if (_activeEvaluatorIndex != i)
                {
                    GUIStyle descStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 10,
                        fontStyle = FontStyle.Italic,
                        wordWrap = true,
                        richText = true
                    };

                    string summary = evaluator.GetEvaluatorSummary();

                    GUILayout.Label(summary, descStyle);
                }

                GUILayout.EndVertical();


                string viewBtn = _activeEvaluatorIndex == i ? "Hide Details" : "View Details";

                if (GUILayout.Button(viewBtn, GUILayout.Width(100), GUILayout.Height(EVALUATOR_LABEL_HEIGHT)))
                {
                    _activeEvaluatorIndex = _activeEvaluatorIndex == i ? -1 : i;
                }

                if (GUILayout.Button("Delete", GUILayout.Width(70), GUILayout.Height(EVALUATOR_LABEL_HEIGHT)))
                {
                    string path = AssetDatabase.GetAssetPath(container);
                    container.RemoveEvaluator(evaluator);
                    DestroyImmediate(evaluator, true);
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(container);
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndHorizontal();

                if (_activeEvaluatorIndex == i)
                {
                    EditorGUILayout.Space(5);

                    if (!_cachedEditors.TryGetValue(evaluator, out var editor))
                    {
                        editor = CreateEditor(evaluator);
                        _cachedEditors[evaluator] = editor;
                    }

                    editor.OnInspectorGUI();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }

        private void OnDisable()
        {
            foreach (var editor in _cachedEditors.Values)
            {
                if (editor != null)
                {
                    DestroyImmediate(editor);
                }
            }

            _cachedEditors.Clear();
        }
    }
}
