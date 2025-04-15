using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    [System.Serializable]
    public class CircleGridGeneratorSettings : GeneratorSettings
    {
        public float OuterRadius = 5f;
        public float InnerRadius = 2f;
        public float Spacing = 0.5f;
        public float OffsetY = 0.5f;
    }
}
