using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Composite", "Parallel Node")]
    public class ParallelNode : ICompositeNode
    {
        public enum ParallelIndex
        {
            Left,
            Right
        }
        [SerializeField] private ParallelIndex m_ReturnValue = ParallelIndex.Right;

        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            NodeState left = Childrens[0].Execute();
            NodeState right = Childrens[1].Execute();
            return m_ReturnValue == ParallelIndex.Right ? right : left;
        }

        protected override void OnExit()
        { }
    }
}