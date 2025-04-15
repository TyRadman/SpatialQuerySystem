using UnityEngine;

namespace SpatialQuery
{
    /// <summary>
    /// A dummy class for custom evaluator subjects. Replace logic here with your own or create a new class that inherits from SpatialQueryEvaluatorSubjectBase
    /// and the result should appear in the evaluator subject list.
    /// </summary>
    [System.Serializable]
    public class CustomEvaluatorSubject : SpatialQueryEvaluatorSubjectBase
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

            return _owner.transform;
        }
    }
}
