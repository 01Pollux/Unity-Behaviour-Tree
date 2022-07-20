using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Always Success")]
    public class AlwaysSuccess : IDecoratorNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            return Child.Execute() != NodeState.Running ? NodeState.Success : NodeState.Running;
        }

        protected override void OnExit()
        { }
    }
}