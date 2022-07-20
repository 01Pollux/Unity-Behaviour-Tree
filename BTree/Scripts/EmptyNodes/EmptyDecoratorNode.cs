using BTree.Core;

namespace BTree.EmptyNodes
{
    [BehaviourNode("Empty Nodes", "Decorator Node")]
    public sealed class EmptyDecoratorNode : IDecoratorNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute() =>
            NodeState.Failure;

        protected override void OnExit()
        { }
    }
}
