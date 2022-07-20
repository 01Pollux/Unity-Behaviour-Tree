using UnityEngine;

namespace BTree.Core
{
    public abstract class IDecoratorNode : INodeBehaviour
    {
        [SerializeField, HideInInspector] private INodeBehaviour[] m_Childrens = new INodeBehaviour[1];
        public INodeBehaviour Child => m_Childrens[0];

        public sealed override INodeBehaviour[] GetChildrens() =>
            m_Childrens;

        public sealed override INodeBehaviour Clone()
        {
            var new_node = Instantiate(this);
            new_node.m_Childrens[0] = Child.Clone();
            return new_node;
        }

        public void SetChild(INodeBehaviour child) =>
            m_Childrens[0] = child;
    }
}