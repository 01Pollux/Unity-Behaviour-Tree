using BTree.Core;

namespace BTree.Nodes
{
    [BehaviourNode("Composite", "Sequence")]
    public class SequenceNode : ICompositeNode
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
            case NodeState.Success:
                if (++m_CurrentChildIdx >= Childrens.Count)
                    return NodeState.Success;
                break;

            case NodeState.Failure:
                return NodeState.Failure;
            }
            return NodeState.Running;
        }

        protected override void OnExit()
        { }
    }
}