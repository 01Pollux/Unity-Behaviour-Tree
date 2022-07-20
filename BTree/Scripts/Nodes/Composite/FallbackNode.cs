using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Composite", "Fallback")]
    public class FallbackNode : ICompositeNode
    {
        private int m_CurrentChildIdx = -1;

        protected override void OnEnter()
        {
            m_CurrentChildIdx = 0;
        }

        protected override NodeState OnExecute()
        {
            switch (Childrens[m_CurrentChildIdx].Execute())
            {
            case NodeState.Failure:
                if (++m_CurrentChildIdx >= Childrens.Count)
                    return NodeState.Failure;
                break;

            case NodeState.Success:
                return NodeState.Success;
            }
            return NodeState.Running;
        }

        protected override void OnExit()
        { }
    }
}