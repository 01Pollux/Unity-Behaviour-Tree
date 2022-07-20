using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Always Failure")]
    public class AlwaysFailure : IDecoratorNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            return Child.Execute() != NodeState.Running ? NodeState.Failure : NodeState.Running;
        }

        protected override void OnExit()
        { }
    }
}