using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        public StringBuilder Report = new StringBuilder();

        internal void ResetPoint()
        {
            IsFilteredOut = false;
            IsAvailable = false;
            Score = 0f;
            IsWinner = false;
        }
    }
}
