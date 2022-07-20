using UnityEngine;

namespace BTree.Core
{
    public abstract class IBehaviourTreeHolder : MonoBehaviour
    {
        [field: SerializeField] public BehaviourTree Tree { get; private set; }

        protected void Initialize(BehaviourTreeBlackboard blackboard = null)
        {
            Tree = Tree.Clone(blackboard);
        }
    }
}