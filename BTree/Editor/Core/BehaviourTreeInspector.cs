using BTreeEditor.Bridge;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace BTreeEditor.Core
{
    public class BehaviourTreeInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeInspector, UxmlTraits>
        { }

        private Editor m_Editor;

        public void UpdateCurrentNode(BehaviourTreeNodeView node_view)
        {
            Clear();

            if (m_Editor)
                Object.DestroyImmediate(m_Editor);

            m_Editor = Editor.CreateEditor(node_view.Node);
            IMGUIContainer container = new(() =>
            {
                if (m_Editor && m_Editor.target)
                {
                    var old_icon = node_view.Node.Icon;
                    m_Editor.OnInspectorGUI();
                    if (old_icon != node_view.Node.Icon)
                        node_view.RefreshIcon();
                }
            });
            Add(container);
        }
    }
}