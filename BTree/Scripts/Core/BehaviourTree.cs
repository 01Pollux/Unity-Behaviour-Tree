using System.Collections.Generic;
using UnityEngine;

namespace BTree.Core
{
    [CreateAssetMenu(fileName = "NewBehaviourTree", menuName = "BTree/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        [field: SerializeField, HideInInspector] public RootNode Root { get; private set; }
        [field: SerializeField, HideInInspector] public NodeState TreeState { get; private set; } = NodeState.Running;

        [field: SerializeField, HideInInspector] public List<INodeBehaviour> Nodes { get; private set; } = new();
        [field: SerializeField, HideInInspector] public BehaviourTreeBlackboard Blackboard { get; private set; }

        public NodeState Execute()
        {
            if (TreeState == NodeState.Running)
            {
                TreeState = Root.Execute();
            }
            return TreeState;
        }

        public void SetRoot(RootNode root) =>
            Root = root;

        public void Rewind()
        {
            TreeState = NodeState.Running;
            Traverse(Root, (node) => node.Rewind());
        }

        public void Abort()
        {
            TreeState = NodeState.Running;
            Root.Abort();
        }


        public BehaviourTree Clone(BehaviourTreeBlackboard blackboard)
        {
            var tree = Instantiate(this);

            tree.Root = Root.Clone() as RootNode;
            tree.Nodes = new();

            if (blackboard == null)
                blackboard = new();

            tree.Blackboard = blackboard;
            blackboard["BTree"] = tree;

            Traverse(tree.Root, n =>
            {
                tree.Nodes.Add(n);
                n.Bind(tree.Blackboard);
                n.Initialize();
                n.Rewind();
            });

            return tree;
        }


        public static void Traverse(INodeBehaviour node, System.Action<INodeBehaviour> predicate)
        {
            List<INodeBehaviour> stack = new();

            var cur_node = node;
            stack.Add(cur_node);

            while (stack.Count > 0)
            {
                cur_node = stack[0];
                stack.RemoveAt(0);

                if (cur_node)
                {
                    predicate(cur_node);
                    var childrens = cur_node.GetChildrens();
                    if (childrens != null)
                        stack.AddRange(childrens);
                }
            }
        }
    }
}