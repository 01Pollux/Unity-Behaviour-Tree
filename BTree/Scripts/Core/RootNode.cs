using UnityEngine;

namespace BTree.Core
{
    [BehaviourNode("Root", "Update")]
    public sealed class RootNode : INodeBehaviour
    {
        [SerializeField, HideInInspector] private INodeBehaviour[] m_Childrens = new INodeBehaviour[1];
        public INodeBehaviour Child => m_Childrens[0];

        protected override void OnEnter()
        { }

        protected override NodeState OnExecute()
        {
            return Child.Execute();
        }

        protected override void OnExit()
        { }


        public override INodeBehaviour[] GetChildrens() =>
            m_Childrens;

        public sealed override INodeBehaviour Clone()
        {
            var new_root = Instantiate(this);
            new_root.m_Childrens[0] = Child.Clone();
            return new_root;
        }

        public void SetChild(INodeBehaviour child) =>
            m_Childrens[0] = child;

    }
}