using BTree.Core;

namespace BTree.EmptyNodes
{
    [BehaviourNode("Empty Nodes", "Composite Node")]
    public sealed class EmptyCompositeNode : ICompositeNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute() =>
            NodeState.Failure;

        protected override void OnExit()
        { }
    }
}
