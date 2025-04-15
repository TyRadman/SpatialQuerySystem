using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialQuery
{
    [System.Serializable]
    public class Test1 : SpatialQueryEvaluatorSubjectBase
    {
        public override void SetUp(GameObject owner)
        {
            base.SetUp(owner);

            Debug.Log("Set up custom evaluator subject");

            // implement any setup logic
        }

        public override Transform GetEvaluatorSubject()
        {
            // implement any custom subject returns

            return null;
        }
    }
}
