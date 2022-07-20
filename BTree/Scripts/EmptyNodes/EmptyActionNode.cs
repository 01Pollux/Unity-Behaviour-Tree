using BTree.Core;

namespace BTree.EmptyNodes
{
    [BehaviourNode("Empty Nodes", "Action Node")]
    public sealed class EmptyActionNode : IActionNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute() =>
            NodeState.Failure;

        protected override void OnExit()
        { }
    }
}
