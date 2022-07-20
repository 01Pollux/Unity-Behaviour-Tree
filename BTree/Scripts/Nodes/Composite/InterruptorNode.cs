using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Composite", "Interruptor")]
    public class InterruptorNode : ICompositeNode
    {
        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            for (int i = 0; i < Childrens.Count; i++)
            {
                var state = Childrens[i].Execute();
                if (state != NodeState.Failure)
                {
                    for (int j = i + 1; j < Childrens.Count; j++)
                        Childrens[j].Abort();
                    return state;
                }
            }
            return NodeState.Failure;
        }

        protected override void OnExit()
        { }
    }
}