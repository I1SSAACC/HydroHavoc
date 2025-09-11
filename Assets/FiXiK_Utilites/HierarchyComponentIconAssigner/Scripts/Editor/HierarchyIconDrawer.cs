#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    public static class HierarchyIconDrawer
    {
        private static readonly Vector2 s_IconSize = new(16, 16);

        private static Dictionary<Type, Texture2D> s_Cache;
        private static HashSet<Type> s_EnabledTypes;
        private static HierarchyIconSettings s_CurrentSettings;
        private static bool s_IsSubscribed;

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded() =>
            EditorApplication.delayCall += Initialize;

        public static void Reload()
        {
            RefreshCache();
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void Initialize()
        {
            RefreshCache();
            Subscribe();
        }

        private static void Subscribe()
        {
            if (s_IsSubscribed)
                return;

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            EditorApplication.quitting += Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload += Unsubscribe;
            ConfigLoader.Config.Changed += OnSettingsChanged;
            s_IsSubscribed = true;
        }

        private static void Unsubscribe()
        {
            if (s_IsSubscribed == false)
                return;

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.quitting -= Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload -= Unsubscribe;
            ConfigLoader.Config.Changed -= OnSettingsChanged;

            if (s_CurrentSettings != null)
                s_CurrentSettings.Changed -= OnSettingsChanged;

            s_IsSubscribed = false;
        }

        private static void RefreshCache()
        {
            if (ConfigLoader.Config == null)
            {
                Debug.LogWarning("Настройки не загружены");
                return;
            }

            if (s_CurrentSettings != ConfigLoader.Config)
            {
                if (s_CurrentSettings != null)
                    s_CurrentSettings.Changed -= OnSettingsChanged;

                s_CurrentSettings = ConfigLoader.Config;
                s_CurrentSettings.Changed += OnSettingsChanged;
            }

            s_EnabledTypes = new();
            s_Cache = new();
            HashSet<Type> seen = new();

            foreach (ComponentIcon param in ConfigLoader.Config.ComponentIconList)
            {
                Type componentType = param.Type;
                Texture2D texture = param.Icon;

                if (componentType == null)
                    continue;

                if (seen.Add(componentType))
                {
                    s_EnabledTypes.Add(componentType);

                    if (texture != null)
                        s_Cache[componentType] = texture;
                }
                else
                {
                    param.Clear();
                }
            }
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (ConfigLoader.Config == null || ConfigLoader.Config.Enabled == false)
                return;

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null)
                return;

            foreach (Type enabledType in s_EnabledTypes)
            {
                if (gameObject.GetComponent(enabledType) != null)
                {
                    if (s_Cache.TryGetValue(enabledType, out Texture2D texture) == false || texture == null)
                        continue;

                    Rect iconRect = new(
                        selectionRect.x,
                        selectionRect.y + selectionRect.height - s_IconSize.y,
                        s_IconSize.x,
                        s_IconSize.y);

                    GUI.DrawTexture(iconRect, texture);
                    break;
                }
            }
        }

        private static void OnSettingsChanged()
        {
            RefreshCache();
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
#endif