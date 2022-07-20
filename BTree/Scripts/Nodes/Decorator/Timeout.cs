using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Timeout")]
    public class Timeout : IDecoratorNode
    {
        [field: SerializeField] public float Duration { get; private set; } = 1f;

        private float m_TimeLeft;

        protected override void OnEnter()
        {
            m_TimeLeft = Duration;
        }

        protected override NodeState OnExecute()
        {
            m_TimeLeft -= Time.deltaTime;
            return m_TimeLeft <= 0f ? NodeState.Failure : Child.Execute();
        }

        protected override void OnExit()
        { }
    }
}