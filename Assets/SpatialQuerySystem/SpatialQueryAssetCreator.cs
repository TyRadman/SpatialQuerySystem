
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    public static class SpatialQueryAssetCreator
    {
        private const string EvaluatorTemplate =
@"using UnityEngine;

namespace SpatialQuery
{                    
    public class CustomEvaluator : SpatialQueryEvaluator
    {
        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            // implement scoring logic here
            // float scoreToAdd = 0.5f;
            // AddToScore(samplePoint, scoreToAdd);
        }

        public override void SetRange(Vector2 generatorRange)
        {
            // If the evaluator relies on a distance or range that's closely tied to the generator's coverage,
            // it's recommended to initialize or adjust it here based on the provided generator range.
        }
    }
}";
        private const string GeneratorTemplate =
@"using UnityEngine;

namespace SpatialQuery
{                    
    public class #SCRIPTNAME# : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            // implement custom logic to generate sample points around the sampling center or any other point in the game

            // base.GenerateSamplePoints(context);
            // Vector3 center = context.GetSamplingCenter();
            // List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();
            // return points;
            return null;
        }
    }
}";        
        private const string EvaluatorSubjectTemplate =
@"using UnityEngine;

namespace SpatialQuery
{                    
    [System.Serializable]
    public class #SCRIPTNAME# : SpatialQueryEvaluatorSubjectBase
    {
        public override void SetUp(GameObject owner)
        {
            base.SetUp(owner);

            // implement setup logic
        }

        public override Transform GetEvaluatorSubject()
        {
            // implement custom subject returns. The returned transform will be the subject of the elevator that has this subject class assigned

            return _owner.transform;
        }
    }
}";


        [MenuItem("Spatial Query/New Spatial Query Asset")]
        public static void CreateScriptableObject()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create ScriptableObject", "NewSpatialQueryAsset", "asset", "Enter a file name");
            
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SpatialQueryAsset asset = ScriptableObject.CreateInstance<SpatialQueryAsset>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Spatial Query/ New Custom Script/Evaluator")]
        public static void CreateEvaluatorScript()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Script", "NewScript", "cs", "Enter a file name");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string className = Path.GetFileNameWithoutExtension(path);
            string content = EvaluatorTemplate.Replace("#SCRIPTNAME#", className);

            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        [MenuItem("Spatial Query/ New Custom Script/Generator")]
        public static void CreateGeneratorScript()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Script", "NewScript", "cs", "Enter a file name");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string className = Path.GetFileNameWithoutExtension(path);
            string content = GeneratorTemplate.Replace("#SCRIPTNAME#", className);

            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        [MenuItem("Spatial Query/ New Custom Script/Evaluator Subject")]
        public static void CreateEvaluatorSubjectScript()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Script", "NewScript", "cs", "Enter a file name");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string className = Path.GetFileNameWithoutExtension(path);
            string content = EvaluatorSubjectTemplate.Replace("#SCRIPTNAME#", className);

            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }
    }
}
