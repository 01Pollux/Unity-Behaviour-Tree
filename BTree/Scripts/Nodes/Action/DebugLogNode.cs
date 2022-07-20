using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Debug", "Debug Log")]
    public class DebugLogNode : IActionNode
    {
        public string Message;

        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            Debug.Log(Message);
            return NodeState.Success;
        }

        protected override void OnExit()
        { }
    }
}