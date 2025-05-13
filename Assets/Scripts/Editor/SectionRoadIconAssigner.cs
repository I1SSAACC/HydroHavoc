#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

[InitializeOnLoad]
public class SectionRoadIconAssigner
{
    private static readonly Dictionary<string, Type> s_imageParams = new Dictionary<string, Type>
    {
        { "network_identity", typeof(NetworkIdentity) },
        { "network_message", typeof(NetworkMessage) },
        { "network_manager", typeof(NetworkManager) }
    };

    private static readonly Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();

    private static bool _isSubscribed = false;

    static SectionRoadIconAssigner()
    {
        Subscribe();
        LoadIcons();
    }

    private static void Subscribe()
    {
        if (!_isSubscribed)
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyItem;
            EditorApplication.quitting += Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload += Unsubscribe;
            _isSubscribed = true;
        }
    }

    private static void Unsubscribe()
    {
        if (_isSubscribed)
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyItem;
            EditorApplication.quitting -= Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload -= Unsubscribe;
            _isSubscribed = false;
        }
    }

    private static void LoadIcons()
    {
        foreach (var entry in s_imageParams)
        {
            s_icons[entry.Key] = LoadIcon(entry.Key);
        }
    }

    private static Texture2D LoadIcon(string iconFileName)
    {
        Texture2D icon = null;

        string[] guids = AssetDatabase.FindAssets($"{iconFileName} t:Texture2D");

        if (guids.Length == 0)
            return icon;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (icon != null)
                    return icon;
            }
        }

        return icon;
    }

    private static void HandleHierarchyItem(int instanceID, Rect rect)
    {
        foreach (var entry in s_imageParams)
        {
            if (s_icons.TryGetValue(entry.Key, out Texture2D icon) && icon != null)
            {
                ShowIcon(instanceID, rect, entry.Value, icon);
            }
        }
    }

    private static void ShowIcon(int instanceID, Rect rect, Type componentType, Texture2D icon)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null) return;

        var component = gameObject.GetComponent(componentType);
        if (component == null) return;

        Rect iconRect = new Rect(rect.x, rect.y, 16f, 16f);
        GUI.DrawTexture(iconRect, icon);
    }
}
#endif
