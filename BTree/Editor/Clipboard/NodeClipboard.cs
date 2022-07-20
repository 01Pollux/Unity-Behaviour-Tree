using BTree.Core;
using BTreeEditor.Bridge;

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;

using UnityEngine;

namespace BTreeEditor.Clipboard
{
    [Serializable]
    public class NodeClipboard : JsonObject
    {
        [SerializeField] private List<INodeBehaviour> m_Nodes = new();
        public List<INodeBehaviour> Nodes => m_Nodes;

        public void AddNode(BehaviourTreeNodeView node_view)
        {
            var cloned_node = INodeBehaviour.Instantiate(node_view.Node);
            EditorUtility.CopySerialized(node_view.Node, cloned_node);
            m_Nodes.Add(cloned_node);
        }
    }
}