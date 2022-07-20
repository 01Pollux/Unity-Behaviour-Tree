using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Loop")]
    public class LoopNode : IDecoratorNode
    {
        [SerializeField, Min(-1)] private int m_LoopCount = 2;
        [SerializeField, Min(-1)] private bool m_ExitOnFailure = false;
        private int m_CurrentLoopCount;

        protected override void OnEnter()
        {
            m_CurrentLoopCount = m_LoopCount;
        }

        protected override NodeState OnExecute()
        {
            if (m_CurrentLoopCount > 0 || m_CurrentLoopCount == -1)
            {
                Child.Execute();

                switch (Child.State)
                {
                case NodeState.Failure:
                    if (m_ExitOnFailure)
                        return NodeState.Failure;
                    goto case NodeState.Success;

                case NodeState.Success:
                    Rewind();
                    if (m_CurrentLoopCount != -1)
                        --m_CurrentLoopCount;
                    break;
                }

                return NodeState.Running;
            }
            return NodeState.Success;
        }

        protected override void OnExit()
        { }


        private new void Rewind()
        {
            BehaviourTree.Traverse(Child, node => node.Rewind());
        }
    }
}