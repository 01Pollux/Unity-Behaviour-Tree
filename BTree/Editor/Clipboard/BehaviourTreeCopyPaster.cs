using BTree.Core;
using BTreeEditor.Bridge;
using BTreeEditor.Core;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BTreeEditor.Clipboard
{
    public class BehaviourTreeCopyPaster
    {
        private BehaviourTreeGraphView m_GraphView;

        public BehaviourTreeCopyPaster(BehaviourTreeGraphView graph_view)
        {
            m_GraphView = graph_view;

            graph_view.serializeGraphElements += CopyCutOperation;
            graph_view.canPasteSerializedData += QueryPasteOperation;
            graph_view.unserializeAndPaste += PasteOperation;
        }


        private string CopyCutOperation(IEnumerable<GraphElement> elements)
        {
            NodeClipboard clipboard = new();

            foreach (var element in elements)
            {
                if (element is BehaviourTreeNodeView node_view)
                    clipboard.AddNode(node_view);
            }

            return JsonUtility.ToJson(clipboard);
        }

        private bool QueryPasteOperation(string data)
        {
            return !string.IsNullOrEmpty(data);
        }

        private void PasteOperation(string operation_name, string data)
        {
            try
            {
                int last_count = m_GraphView.TreeBridge.NodesCount;

                var linked_nodes = InsertNodes(data);
                ResolveLinkedNodes(linked_nodes);

                m_GraphView.TreeBridge.LinkEdges(last_count);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private Dictionary<string, BehaviourNodeCloneInfo> InsertNodes(string data)
        {
            var clipboard = JsonUtility.FromJson<NodeClipboard>(data);
            Dictionary<string, BehaviourNodeCloneInfo> linked_nodes = new();

            foreach (var node in clipboard.Nodes)
            {
                var type = node.GetType();
                var attribute = type.GetCustomAttribute<BehaviourNodeAttribute>();

                var new_node = m_GraphView.TreeBridge.CreateNode(type, attribute, false);
                linked_nodes.Add(node.Guid, new BehaviourNodeCloneInfo() { Parent = node, Clone = new_node });

                var guid = new_node.Node.Guid;
                EditorUtility.CopySerialized(node, new_node.Node);
                new_node.Node.SetGUID(guid);

                new_node.SetPosition(node.Position + new Vector2(-140f, -40f));
            }

            return linked_nodes;
        }

        private void ResolveLinkedNodes(Dictionary<string, BehaviourNodeCloneInfo> linked_nodes)
        {
            foreach (var (guid, clone_info) in linked_nodes)
            {
                var cloned_node = clone_info.Clone.Node;

                if (cloned_node is IActionNode)
                    continue;
                else if (cloned_node is ICompositeNode composite)
                {
                    composite.RemoveChildrens();
                    var childrens = clone_info.Parent.GetChildrens();

                    foreach (var child in childrens)
                    {
                        if (linked_nodes.ContainsKey(child.Guid))
                            composite.AddChild(linked_nodes[child.Guid].Clone.Node);
                    }
                }
                else if (cloned_node is IDecoratorNode decorator)
                {
                    decorator.SetChild(null);
                    if (linked_nodes.ContainsKey(guid))
                        decorator.SetChild(linked_nodes[guid].Clone.Node);
                }
                else if (cloned_node is RootNode root)
                {
                    root.SetChild(null);
                    if (linked_nodes.ContainsKey(guid))
                        root.SetChild(linked_nodes[guid].Clone.Node);
                }
            }
        }

        struct BehaviourNodeCloneInfo
        {
            public INodeBehaviour Parent;
            public BehaviourTreeNodeView Clone;
        }
    }
}
