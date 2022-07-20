using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Action", "Behaviour Tree Reference")]
    public class BehaviourTreeRef : IActionNode
    {
        [SerializeField] private BehaviourTree m_Tree;
        [SerializeField] private bool m_SharedBlackboard = true;

        public override void Initialize()
        {
            m_Tree = m_Tree.Clone(m_Tree.Blackboard);
        }

        public override INodeBehaviour Clone()
        {
            BehaviourTreeRef clone = Instantiate(this);
            clone.m_Tree = m_Tree.Clone(m_SharedBlackboard ? Blackboard : null);
            return clone;
        }

        protected override void OnEnter()
        {
            m_Tree.Rewind();
        }

        protected override NodeState OnExecute()
        {
            return m_Tree.Execute();
        }

        protected override void OnExit()
        { }

        public override void Rewind()
        {
            base.Rewind();
            m_Tree.Rewind();
        }
    }
}
