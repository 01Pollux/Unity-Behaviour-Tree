using BTree.Core;

using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace BTreeEditor.Bridge
{
    public class BehaviourTreeBridge
    {
        public static BehaviourTree activeTree
        {
            get
            {
                var tree = Selection.activeObject as BehaviourTree;
                if (!tree)
                {
                    if (Selection.activeGameObject)
                    {
                        var holder = Selection.activeGameObject.GetComponent<IBehaviourTreeHolder>();
                        if (holder)
                            tree = holder.Tree;
                    }
                }
                else if (!Application.isPlaying && !AssetDatabase.Contains(tree))
                    tree = null;

                return tree;
            }
        }

        internal void Reset()
        {
            Tree = null;
            m_Nodes.Clear();
        }


        public BehaviourTree Tree { get; private set; }

        private List<BehaviourTreeNodeView> m_Nodes = new();
        public int NodesCount => m_Nodes.Count;

        private GraphView m_Graphview;

        public System.Action<BehaviourTreeNodeView> OnNodeSelected;


        public BehaviourTreeBridge(GraphView graph)
        {
            m_Graphview = graph;
        }

        public void Initialize(BehaviourTree tree, bool doundo_state)
        {
            Tree = tree;
            m_Nodes.Clear();

            if (Tree.Root == null)
            {
                Tree.SetRoot(CreateNode(typeof(RootNode), "Update", doundo_state).Node as RootNode);
                EditorUtility.SetDirty(Tree);
                AssetDatabase.SaveAssets();
            }
            else
                LoadAssets(doundo_state);
        }

        private void LoadAssets(bool doundo_state)
        {
            for (int i = 0; i < Tree.Nodes.Count; i++)
            {
                var node = Tree.Nodes[i];
                if (node == null)
                    Tree.Nodes.RemoveAt(i);
                else
                {
                    if (node is ICompositeNode composite)
                    {
                        var childrens = composite.GetChildrens();
                        composite.RemoveNullChildrens();
                    }
                    CreateNode(node, doundo_state);
                }
            }

            LinkEdges(0);

            AssetDatabase.Refresh();
        }


        private BehaviourTreeNodeView FindNodeView(INodeBehaviour node)
        {
            foreach (var node_view in m_Graphview.nodes)
            {
                var btnode_view = node_view as BehaviourTreeNodeView;
                if (btnode_view.Node.Guid.Equals(node.Guid))
                    return btnode_view;
            }
            return null;
        }


        internal void AddChild(INodeBehaviour parent, INodeBehaviour child)
        {
            if (parent is IDecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Remove child)");
                decorator.SetChild(child);
                EditorUtility.SetDirty(decorator);
            }
            else if (parent is ICompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Add child)");
                composite.AddChild(child);
                EditorUtility.SetDirty(composite);
            }
            else if (parent is RootNode root)
            {
                Undo.RecordObject(root, "Behaviour Tree (Add child)");
                root.SetChild(child);
                EditorUtility.SetDirty(root);
            }
            else
                return;
            AssetDatabase.Refresh();
        }

        internal void RemoveChild(INodeBehaviour parent, INodeBehaviour child)
        {
            if (parent is IDecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (Remove child)");
                decorator.SetChild(null);
                EditorUtility.SetDirty(decorator);
            }
            else if (parent is ICompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (Remove child)");
                composite.RemoveChild(child);
                EditorUtility.SetDirty(composite);
            }
            else if (parent is RootNode root)
            {
                Undo.RecordObject(root, "Behaviour Tree (Remove child)");
                root.SetChild(null);
                EditorUtility.SetDirty(root);
            }
            else
                return;
            AssetDatabase.Refresh();
        }


        internal BehaviourTreeNodeView CreateNode(System.Type type, BehaviourNodeAttribute attribute, bool doundo_state) =>
            CreateNode(type, attribute.Name, doundo_state);


        internal BehaviourTreeNodeView CreateNode(System.Type type, string name, bool doundo_state)
        {
            if (!doundo_state)
                Undo.RecordObject(Tree, "Behaviour Tree (Create Node)");

            BehaviourTreeNodeView node_view = new(type, name);
            Tree.Nodes.Add(node_view.Node);

            if (!doundo_state)
            {
                Undo.RegisterCreatedObjectUndo(node_view.Node, "Behaviour Tree (Create Node)");
            }

            m_Nodes.Add(node_view);
            node_view.SetupView(type, m_Graphview);

            node_view.OnNodeSelected += OnNodeSelected;

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node_view.Node, Tree);
            AssetDatabase.SaveAssets();

            return node_view;
        }

        internal BehaviourTreeNodeView CreateNode(INodeBehaviour node, bool doundo_state)
        {
            if (!doundo_state)
                Undo.RecordObject(Tree, "Behaviour Tree (Create Node)");

            BehaviourTreeNodeView node_view = new(node);

            if (!doundo_state)
                Undo.RegisterCreatedObjectUndo(node_view.Node, "Behaviour Tree (Create Node)");

            m_Nodes.Add(node_view);
            node_view.SetupView(node.GetType(), m_Graphview);

            node_view.OnNodeSelected += OnNodeSelected;

            AssetDatabase.SaveAssets();

            return node_view;
        }


        internal void RemoveNode(BehaviourTreeNodeView node_view)
        {
            Undo.RecordObject(Tree, "Behaviour Tree (Remove Node)");

            Tree.Nodes.Remove(node_view.Node);
            m_Nodes.Remove(node_view);

            Undo.DestroyObjectImmediate(node_view.Node);

            node_view.OnNodeSelected -= OnNodeSelected;

            AssetDatabase.SaveAssets();
        }


        internal void Sort()
        {
            foreach (BehaviourTreeNodeView node in m_Nodes)
            {
                if (node.Node is ICompositeNode composite)
                    composite.Sort();
            }
        }

        internal void UpdateState()
        {
            m_Nodes.ForEach(n =>
            {
                n.UpdateState();
            });
        }


        internal void LinkEdges(int start_position)
        {
            for (int i = start_position; i < Tree.Nodes.Count; i++)
            {
                var parent = Tree.Nodes[i];
                var childrens = parent.GetChildrens();
                if (childrens == null)
                    continue;

                var parent_view = FindNodeView(parent);
                foreach (var child in childrens)
                {
                    var child_view = child ? FindNodeView(child) : null;
                    if (child_view == null)
                        continue;

                    Edge edge = parent_view.OuputPort.ConnectTo(child_view.InputPort);
                    m_Graphview.AddElement(edge);
                }
            }
        }
    }
}