using Assets.Plugins.BTree.Editor.Bridge;
using BTree.Core;

using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;


namespace BTreeEditor.Bridge
{
    public class BehaviourTreeNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public INodeBehaviour Node { get; private set; }

        public string Name => title;
        public string Description => Node.Description;

        public Port InputPort { get; private set; }
        public Port OuputPort { get; private set; }

        internal System.Action<BehaviourTreeNodeView> OnNodeSelected;
        private StyleBackground m_OldbackIcon;


        internal BehaviourTreeNodeView(System.Type type, string node_name) :
            base(GetNodeStyleSheetPath()) =>
            InitializeNode(ScriptableObject.CreateInstance(type) as INodeBehaviour, type);

        internal BehaviourTreeNodeView(INodeBehaviour node) :
            base(GetNodeStyleSheetPath()) =>
            InitializeNode(node, node.GetType());


        internal void InitializeNode(INodeBehaviour node, System.Type type)
        {
            var attribute = type.GetCustomAttribute<BehaviourNodeAttribute>();

            Node = node;
            Node.name = type.Name;
            if (Node.Guid == null)
                Node.SetGUID(GUID.Generate().ToString());

            style.left = Node.Position.x;
            style.top = Node.Position.y;

            title = attribute.Name;

            m_OldbackIcon = this.Q("node-icon").style.backgroundImage;
            RefreshIcon();

            var description = this.Q<Label>("node-description");
            description.bindingPath = "Description";
            description.Bind(new SerializedObject(Node));
        }


        internal void SetupView(System.Type type, GraphView graph)
        {
            if (type != typeof(RootNode))
            {
                while (!type.IsAbstract)
                    type = type.BaseType;
            }
            if (NodeDescriptor.ContainsKey(type))
            {
                var descriptor = NodeDescriptor[type];

                if (descriptor.HasInput)
                {
                    InputPort = new BehaviourTreeNodePort(Direction.Input, descriptor.InputCapacity);
                    InputPort.portName = "";
                    inputContainer.Add(InputPort);
                }

                if (descriptor.HasOuput)
                {
                    OuputPort = new BehaviourTreeNodePort(Direction.Output, descriptor.OuputCapacity);
                    OuputPort.portName = "";
                    outputContainer.Add(OuputPort);
                }

                foreach (var classname in descriptor.ClassNames)
                    AddToClassList(classname);
            }

            graph.AddElement(this);
        }


        public override void SetPosition(Rect new_pos)
        {
            base.SetPosition(new_pos);
            Node.SetPosition(new_pos.position);
        }


        internal void SetPosition(Vector2 position)
        {
            Node.SetPosition(position);
            style.left = position.x;
            style.top = position.y;
        }


        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        internal void UpdateState()
        {
            ResetState();
            switch (Node.State)
            {
            case NodeState.Success:
                AddToClassList("btstate-success");
                break;
            case NodeState.Failure:
                AddToClassList("btstate-failure");
                break;
            case NodeState.Running:
                if (Node.Started)
                    AddToClassList("btstate-running");
                break;
            }
        }

        internal void ResetState()
        {
            RemoveFromClassList("btstate-success");
            RemoveFromClassList("btstate-failure");
            RemoveFromClassList("btstate-running");
        }

        internal void RefreshIcon()
        {
            var node_icon = this.Q("node-icon").style;
            var node_title = this.Q("title").style;

            if (UI.BehaviourTreeSettings.DisplayIcon)
            {
                node_icon.display = DisplayStyle.Flex;
                node_title.display = DisplayStyle.None;

                if (Node.Icon != null)
                    node_icon.backgroundImage = new StyleBackground(Node.Icon);
                else
                    node_icon.backgroundImage = m_OldbackIcon;
            }
            else
            {
                node_icon.display = DisplayStyle.None;
                node_title.display = DisplayStyle.Flex;
            }
        }


        private static string GetNodeStyleSheetPath() =>
            AssetDatabase.GetAssetPath(UI.BehaviourTreeSettings.NodeStyleSheet);

        private static readonly Dictionary<System.Type, BehaviourTreeNodeDescriptor> NodeDescriptor = new()
        {
            { typeof(RootNode), new BehaviourTreeNodeDescriptor()
                { ClassNames = new string[]{ "btnode-root" }, HasInput = false, OuputCapacity = Port.Capacity.Single } },

            { typeof(IActionNode), new BehaviourTreeNodeDescriptor()
                { ClassNames = new string[]{ "btnode-action" }, InputCapacity = Port.Capacity.Single, HasOuput = false } },

            { typeof(ICompositeNode), new BehaviourTreeNodeDescriptor()
                { ClassNames = new string[]{ "btnode-composite" }, InputCapacity = Port.Capacity.Single, OuputCapacity = Port.Capacity.Multi } },

            { typeof(IDecoratorNode), new BehaviourTreeNodeDescriptor()
                { ClassNames = new string[]{ "btnode-decorator" }, InputCapacity = Port.Capacity.Single, OuputCapacity = Port.Capacity.Single } }
        };
    }
}