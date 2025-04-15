namespace SpatialQuery
{
    [System.Serializable]
    public class ConeGeneratorSettings : GeneratorSettings 
    {
        public float InnerRadius = 30f;
        public float OuterRadius = 90f;
        public int RingCount = 3;
        public int PointsPerRing = 24;
        public float Angle = 45f;
        public float OffsetY = 0.5f;
    }
}
