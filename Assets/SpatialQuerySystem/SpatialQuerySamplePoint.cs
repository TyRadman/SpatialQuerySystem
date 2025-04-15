using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    public class SpatialQuerySamplePoint
    {
        public Vector3 PointPosition;
        public float Score;
        public bool IsFilteredOut = false;
        public bool IsAvailable = true;

        public bool IsWinner = false;

        internal void ResetPoint()
        {
            IsFilteredOut = false;
            IsAvailable = false;
            Score = 0f;
            IsWinner = false;
        }
    }
}
