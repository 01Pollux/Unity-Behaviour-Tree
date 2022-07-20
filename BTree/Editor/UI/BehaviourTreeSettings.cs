using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace BTreeEditor.UI
{
    public class BehaviourTreeSettings : ScriptableObject
    {
        public VisualTreeAsset m_EditorUXML;
        public static VisualTreeAsset EditorUXML => Instance.m_EditorUXML;

        public StyleSheet m_EditorStyleSheet;
        public static StyleSheet EditorStyleSheet => Instance.m_EditorStyleSheet;

        public VisualTreeAsset m_NodeStyleSheet;
        public static VisualTreeAsset NodeStyleSheet => Instance.m_NodeStyleSheet;

        public bool m_DisplayIcon;
        public static bool DisplayIcon => Instance.m_DisplayIcon;

        public static bool IsValid
        {
            get
            {
                var settings = Instance;
                if (!settings.m_NodeStyleSheet || !settings.m_EditorStyleSheet || !settings.m_EditorUXML)
                {
                    Debug.LogError("Behaviour tree's settings is missing a component(s), Check Project settings.");
                    return false;
                }
                return true;
            }
        }


        internal static BehaviourTreeSettings Instance => s_Instance.Value;
        internal static SerializedObject SerializedInstance => new(Instance);


        private readonly static Lazy<BehaviourTreeSettings> s_Instance = new(() =>
        {
            var settings = FindSettings();
            if (!settings)
            {
                settings = CreateInstance<BehaviourTreeSettings>();
                settings.name = "BehaviourTreeSettings";
                AssetDatabase.CreateAsset(settings, "Assets/Settings/BTree.asset");
                AssetDatabase.SaveAssets();
            }

            return settings;
        });


        private static BehaviourTreeSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets("t:BehaviourTreeSettings");

            if (guids.Length > 1)
                Debug.LogWarning("Multiple instances of 'BehaviourTreeSettings' was found in the project.");

            switch (guids.Length)
            {
            case 0:
                return null;

            default:
                return AssetDatabase.LoadAssetAtPath<BehaviourTreeSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
        }
    }


    public static class BehaviourTreeEditorSettings
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider(
                path: "Project/BehaviourTree",
                scopes: SettingsScope.Project)
            {
                label = "Behaviour Tree",
                activateHandler = (search_context, root_element) =>
                {
                    var title = new Label()
                    {
                        text = "Behaviour Tree Settings"
                    };
                    title.AddToClassList("title");
                    root_element.Add(title);


                    var properties = new VisualElement()
                    {
                        style = { flexDirection = FlexDirection.Column }
                    };
                    properties.AddToClassList("property-list");
                    root_element.Add(properties);

                    var settings = BehaviourTreeSettings.SerializedInstance;
                    root_element.Add(new InspectorElement(settings));

                    root_element.Bind(settings);
                },
                keywords = new string[] { "Behaviour", "Tree", "BTree" }
            };
        }
    }
}
