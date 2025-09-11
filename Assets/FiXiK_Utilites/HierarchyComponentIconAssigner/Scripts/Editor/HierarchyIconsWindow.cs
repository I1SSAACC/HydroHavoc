#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    public class HierarchyIconsWindow : EditorWindow
    {
        private const string MenuPath = "Tools/";
        private const string WindowTitle = "Иконки в иерархии сцены";
        private const float Space = 10;
        private static readonly Vector2 s_MinSizeWindow = new(300, 200);

        private SerializedObject _serializedObject;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            if (ConfigLoader.Config != null)
            {
                _serializedObject = new(ConfigLoader.Config);
                ConfigLoader.Config.Changed += OnChangesSettings;
            }
        }

        private void OnDisable()
        {
            if (ConfigLoader.Config != null)
                ConfigLoader.Config.Changed -= OnChangesSettings;
        }

        private void OnChangesSettings()
        {
            HierarchyIconDrawer.Reload();
            _serializedObject.Update();
            Repaint();
            EditorUtility.SetDirty(ConfigLoader.Config);
        }

        private void OnGUI()
        {
            GUILayout.Space(Space);

            if (ConfigLoader.Config == null || _serializedObject == null)
            {
                EditorGUILayout.HelpBox($"Не удалось создать или найти {nameof(HierarchyIconSettings)}", MessageType.Error);
                return;
            }

            _serializedObject.Update();

            EditorGUILayout.PropertyField(_serializedObject.FindProperty(HierarchyIconSettings.EnablerPropertyName));

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.PropertyField(_serializedObject.FindProperty(HierarchyIconSettings.ComponentIconListPropertyName), true);
            EditorGUILayout.EndScrollView();

            if (_serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(ConfigLoader.Config);
        }

        [MenuItem(MenuPath + WindowTitle)]
        public static void ShowWindow()
        {
            HierarchyIconsWindow window = GetWindow<HierarchyIconsWindow>(WindowTitle);
            window.minSize = s_MinSizeWindow;
            window.Show();
        }
    }
}
#endif