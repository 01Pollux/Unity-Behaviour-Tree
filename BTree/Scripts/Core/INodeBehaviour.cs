using UnityEngine;

namespace BTree.Core
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }

    public abstract class INodeBehaviour : ScriptableObject
    {
        [field: SerializeField, HideInInspector] public NodeState State { get; private set; } = NodeState.Running;
        [field: SerializeField, HideInInspector] public bool Started { get; private set; } = false;
        [field: SerializeField, HideInInspector] public BehaviourTreeBlackboard Blackboard { get; private set; }

#if UNITY_EDITOR
        [SerializeField, TextArea] public string Description;
        [field: SerializeField, HideInInspector] public string Guid { get; private set; }
        [field: SerializeField, HideInInspector] public Vector2 Position { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
#endif


        public NodeState Execute()
        {
            if (!Started)
            {
                OnEnter();
                Started = true;
            }

            State = OnExecute();

            if (State != NodeState.Running)
            {
                Started = false;
                OnExit();
            }

            return State;
        }

        public void Abort()
        {
            BehaviourTree.Traverse(this,
                predicate: node =>
                {
                    if (node.Started)
                        node.OnExit();
                    node.Rewind();
                }
            );
        }


        public void Bind(BehaviourTreeBlackboard blackboard) =>
            Blackboard = blackboard;


        public virtual void Initialize() { }

        protected abstract void OnEnter();
        protected abstract NodeState OnExecute();
        protected abstract void OnExit();

        public virtual void Rewind()
        {
            Started = false;
            State = NodeState.Running;
        }

        public abstract INodeBehaviour[] GetChildrens();
        public virtual INodeBehaviour Clone() =>
            Instantiate(this);


#if UNITY_EDITOR
        public void SetGUID(string guid) =>
            Guid = guid;

        public void SetPosition(Vector2 position)
        {
            UnityEditor.Undo.RecordObject(this, "Behaviour Tree (Set Position)");
            Position = position;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}