using UnityEngine;

namespace SpatialQuery
{
    public static class SpatialQueryUtils
    {
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
    }
}