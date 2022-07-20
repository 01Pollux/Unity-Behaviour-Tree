using System.Collections.Generic;
using UnityEngine;

namespace BTree.Core
{
    public abstract class ICompositeNode : INodeBehaviour
    {
        [field: SerializeField, HideInInspector] protected List<INodeBehaviour> Childrens { get; set; } = new();

        public sealed override INodeBehaviour[] GetChildrens() =>
            Childrens.ToArray();

        public sealed override INodeBehaviour Clone()
        {
            var new_node = Instantiate(this);
            new_node.Childrens = Childrens.ConvertAll(n => n.Clone());
            return new_node;
        }


        public void AddChild(INodeBehaviour child) =>
            Childrens.Add(child);

        public void RemoveChild(INodeBehaviour child) =>
            Childrens.Remove(child);

#if UNITY_EDITOR
        public void Sort() =>
            Childrens.Sort((lhs, rhs) => lhs.Position.x < rhs.Position.x ? -1 : 1);

        public void RemoveNullChildrens()
        {
            for(int i = 0; i < Childrens.Count; i++)
            {
                if (Childrens[i] == null)
                    Childrens.RemoveAt(i);
            }
        }

        public void RemoveChildrens() =>
            Childrens.Clear();
#endif
    }
}