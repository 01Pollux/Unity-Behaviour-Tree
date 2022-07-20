using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Debug", "Breakpoint")]
    public class BreakPoint : IActionNode
    {
        protected override void OnEnter()
        {
            Debug.Break();
        }

        protected override NodeState OnExecute()
        {
            return NodeState.Success;
        }

        protected override void OnExit()
        { }
    }
}