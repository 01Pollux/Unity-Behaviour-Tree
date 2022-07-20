using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Timers", "Wait")]
    public class WaitNode : IActionNode
    {
        [SerializeField] private float m_Duration = 2.0f;
        private float m_CurTime = 0;

        protected override void OnEnter()
        {
            m_CurTime = m_Duration;
        }

        protected override NodeState OnExecute()
        {
            m_CurTime -= Time.deltaTime;
            return m_CurTime <= 0f ? NodeState.Success : NodeState.Running;
        }

        protected override void OnExit()
        { }
    }
}