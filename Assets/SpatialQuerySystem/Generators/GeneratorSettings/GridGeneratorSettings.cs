using UnityEngine;

namespace SpatialQuery
{
    /// <summary>
    /// Data container for the settings of Spatial Query Grid Generators.
    /// </summary>
    [System.Serializable]
    public class GridGeneratorSettings : GeneratorSettings
    {
        public float CoverageSize = 5f;
        public float Spacing = 0.5f;
        public float OffsetY = 0.5f;
    }
}
