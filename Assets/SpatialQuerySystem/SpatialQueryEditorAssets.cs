using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpatialQuery
{
    [InitializeOnLoad]
    public class SpatialQueryEditorAssets : MonoBehaviour
    {
        public static string PluginRootPath;

        public static Texture2D LoadIcon(string fileName)
        {
            string[] guids = AssetDatabase.FindAssets("T_Arrow_Up t:Texture2D");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                PluginRootPath = Path.GetDirectoryName(path);
            }
            else
            {
                Debug.LogError("Could not locate SpatialQuery plugin assets folder.");
            }

            if (string.IsNullOrEmpty(PluginRootPath))
            {
                return null;
            }

            string fullPath = Path.Combine(PluginRootPath, fileName);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
        }

        public static SpatialQueryDebugPoint LoadDebugSamplePointPrefab()
        {
            string[] guids = AssetDatabase.FindAssets("PF_SpatialQueryDebugPoint t:Prefab");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                return prefab.GetComponent<SpatialQueryDebugPoint>();
            }

            return null;
        }
    }
}
