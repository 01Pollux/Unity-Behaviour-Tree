using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Inverter")]
    public class Inverter : IDecoratorNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            switch (Child.Execute())
            {
            case NodeState.Success:
                return NodeState.Failure;
            case NodeState.Failure:
                return NodeState.Success;
            default:
                return NodeState.Running;
            }
        }

        protected override void OnExit()
        { }
    }
}