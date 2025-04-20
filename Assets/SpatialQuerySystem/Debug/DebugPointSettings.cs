using UnityEngine;

namespace SpatialQuery
{
    public class DebugPointSettings
    {
        public SpatialQuerySamplePoint Point;
        public float Duration = 1f;
        public ScoreDisplayMode ScoreDisplayMode;
        public Vector2 ScalingRange = new Vector2(0.25f, 1.5f);
        public bool UseDefaultGradient = true;
        public Gradient ScoreColorGradient;
        public bool Animate = true;
        public bool HidePointsAfterShowing = true;
    }
}
