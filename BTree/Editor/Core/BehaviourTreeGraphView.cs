using BTree.Core;
using BTreeEditor.Bridge;
using BTreeEditor.Clipboard;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;


namespace BTreeEditor.Core
{
    public class BehaviourTreeGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, UxmlTraits>
        { }

        private Dictionary<string, List<BehaviourNodeTypeInfo>> m_TypeCache = new();
        private BehaviourTreeCopyPaster m_CopyPaster;

        public BehaviourTreeBridge TreeBridge { get; private set; }

        public BehaviourTreeGraphView()
        {
            TreeBridge = new(this);

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var style = UI.BehaviourTreeSettings.EditorStyleSheet;
            if (style)
                styleSheets.Add(style);

            BuildTypeCache();
            Undo.undoRedoPerformed += OnUndoRedo;

            m_CopyPaster = new(this);
        }


        public void InitializeTree()
        {
            var tree = BehaviourTreeBridge.activeTree;
            if (!tree)
                return;

            if (TreeBridge.Tree != tree)
            {
                DeleteElements();
                TreeBridge.Initialize(tree, false);

                EditorApplication.delayCall += () => FrameAll();;
            }
        }

        private void OnUndoRedo()
        {
            if (TreeBridge != null)
            {
                DeleteElements();
                TreeBridge.Initialize(TreeBridge.Tree, true);
                AssetDatabase.SaveAssets();
            }
        }

        private void DeleteElements()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!TreeBridge.Tree)
                return;

            Vector2 position = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

            foreach (var (category, typeinfos) in m_TypeCache)
            {
                foreach (var typeinfo in typeinfos)
                {
                    evt.menu.AppendAction(
                        $"{category}/{typeinfo.Attribute.Name}",
                        (a) =>
                        {
                            var node_view = TreeBridge.CreateNode(typeinfo.Type, typeinfo.Attribute, false);
                            node_view.SetPosition(position);
                        }
                        );
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port start_port, NodeAdapter adapter)
        {
            return ports.ToList().Where(end_port =>
                end_port.direction != start_port.direction &&
                    end_port.node != start_port.node
            ).ToList();
        }


        private GraphViewChange OnGraphViewChanged(GraphViewChange graph_view)
        {
            if (graph_view.elementsToRemove != null)
            {
                graph_view.elementsToRemove.ForEach(element =>
                {
                    if (element is BehaviourTreeNodeView node_view)
                        TreeBridge.RemoveNode(node_view);
                    else if (element is Edge edge)
                    {
                        BehaviourTreeNodeView parent_view = edge.output.node as BehaviourTreeNodeView;
                        BehaviourTreeNodeView child_view = edge.input.node as BehaviourTreeNodeView;
                        TreeBridge.RemoveChild(parent_view.Node, child_view.Node);
                    }
                });
            }

            if (graph_view.edgesToCreate != null)
            {
                graph_view.edgesToCreate.ForEach(edge =>
                {
                    BehaviourTreeNodeView parent_view = edge.output.node as BehaviourTreeNodeView;
                    BehaviourTreeNodeView child_view = edge.input.node as BehaviourTreeNodeView;
                    TreeBridge.AddChild(parent_view.Node, child_view.Node);
                });
            }

            TreeBridge.Sort();

            return graph_view;
        }


        private void BuildTypeCache()
        {
            var types = TypeCache.GetTypesWithAttribute<BehaviourNodeAttribute>();

            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(INodeBehaviour)) && !type.IsAbstract)
                {
                    var attr = type.GetCustomAttribute<BehaviourNodeAttribute>();
                    if (!m_TypeCache.ContainsKey(attr.Category))
                        m_TypeCache.Add(attr.Category, new List<BehaviourNodeTypeInfo>());

                    m_TypeCache[attr.Category].Add(new() { Type = type, Attribute = attr });
                }
            }
        }

        public struct BehaviourNodeTypeInfo
        {
            public Type Type;
            public BehaviourNodeAttribute Attribute;
        }
    }
}
