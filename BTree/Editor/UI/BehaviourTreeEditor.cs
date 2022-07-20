using BTree.Core;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace BTreeEditor.UI
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private Core.BehaviourTreeInspector m_Inspector;
        private Core.BehaviourTreeGraphView m_Graphview;

        [MenuItem("Tools/Behaviour Tree Editor...")]
        public static void OpenWindow()
        {
            if (BehaviourTreeSettings.IsValid)
            {
                BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
                wnd.titleContent = new GUIContent("Behaviour Tree Editor");
                wnd.Focus();
            }
        }

        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instance_id, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = BehaviourTreeSettings.EditorUXML;
            visualTree.CloneTree(root);

            var styleSheets = BehaviourTreeSettings.EditorStyleSheet;
            root.styleSheets.Add(styleSheets);

            m_Inspector = root.Q<Core.BehaviourTreeInspector>();
            m_Graphview = root.Q<Core.BehaviourTreeGraphView>();

            m_Graphview.TreeBridge.OnNodeSelected = m_Inspector.UpdateCurrentNode;

            OnSelectionChange();
        }

        public void OnSelectionChange()
        {
            if (m_Graphview != null)
                EditorApplication.delayCall += () => { m_Graphview.InitializeTree(); };
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying && m_Graphview?.TreeBridge != null)
                m_Graphview.TreeBridge.UpdateState();
        }

        void OnFocus() => OnSelectionChange();

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            m_Graphview?.TreeBridge.Reset();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
            case PlayModeStateChange.EnteredEditMode:
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            }
        }
    }
}