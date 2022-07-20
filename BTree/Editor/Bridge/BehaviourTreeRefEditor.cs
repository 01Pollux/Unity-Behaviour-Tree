using BTreeEditor.UI;
using UnityEditor;
using UnityEngine;

namespace BTreeEditor.Bridge
{
    [CustomEditor(typeof(BTree.Nodes.BehaviourTreeRef))]
    public class BehaviourTreeRefEditor : Editor
    {
        private SerializedProperty m_TreeProp;

        private void OnValidate()
        {
            try
            {
                if (serializedObject != null)
                    m_TreeProp = serializedObject.FindProperty("m_Tree");
            }
            catch
            {
                m_TreeProp = null;
            }
        }

        public override void OnInspectorGUI()
        {
            if (m_TreeProp == null)
                return;

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_TreeProp);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Open Tree", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
            {
                var editor = EditorWindow.GetWindow<BehaviourTreeEditor>();
                if (editor)
                {
                    Selection.activeObject = m_TreeProp.objectReferenceValue;
                    editor.OnSelectionChange();
                }
            }
        }
    }
}
