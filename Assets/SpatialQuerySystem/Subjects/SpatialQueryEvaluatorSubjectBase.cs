using UnityEngine;

namespace SpatialQuery
{
    /// <summary>
    /// Base class for subjects that evaluators perform checks against.
    /// </summary>
    [System.Serializable]
    public abstract class SpatialQueryEvaluatorSubjectBase : ScriptableObject
    {
        protected GameObject _owner;

        public virtual void SetUp(GameObject owner)
        {
            _owner = owner;
        }

        public abstract Transform GetEvaluatorSubject();
    }
}
