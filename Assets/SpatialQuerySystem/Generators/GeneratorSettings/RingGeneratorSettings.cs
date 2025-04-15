namespace SpatialQuery
{
    [System.Serializable]
    public class RingGeneratorSettings : GeneratorSettings
    {
        public float InnerRadius = 5f;
        public float OuterRadius = 20f;
        public int RingCount = 4;
        public int PointsPerRing = 12;
        public float OffsetY = 0.5f;
    }
}
