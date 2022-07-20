using BTree.Core;
using UnityEngine;

namespace BTree.Nodes
{
    [BehaviourNode("Decorator", "Converter")]
    public class ConverterNode : IDecoratorNode
    {
        [SerializeField] private NodeState 
            m_SuccessResult = NodeState.Success, 
            m_FailureResult = NodeState.Failure,
            m_RunningResult = NodeState.Running;

        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            switch (Child.Execute())
            {
            case NodeState.Success:
                return m_SuccessResult;
            case NodeState.Failure:
                return m_FailureResult;
            default:
                return m_RunningResult;
            }
        }

        protected override void OnExit()
        { }
    }
}
