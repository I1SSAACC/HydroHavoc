using UnityEditor;
using UnityEngine;
using System.IO;

public class PrefabMenu : EditorWindow
{
    private string[] prefabPaths;
    private double lastUpdateTime;

    [MenuItem("Tools/Prefab Manager")]
    public static void ShowWindow() => GetWindow<PrefabMenu>("Prefab Manager");

    private void OnEnable()
    {
        LoadPrefabs();
        lastUpdateTime = EditorApplication.timeSinceStartup;
    }

    private void LoadPrefabs()
    {
        string prefabFolder = "Assets/Prefabs";
        if (Directory.Exists(prefabFolder))
        {
            prefabPaths = Directory.GetFiles(prefabFolder, "*.prefab");
        }
        else
        {
            Debug.LogWarning($"Directory not found: {prefabFolder}");
            prefabPaths = new string[0];
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

        if (prefabPaths != null && prefabPaths.Length > 0)
        {
            foreach (var prefabPath in prefabPaths)
            {
                string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                if (GUILayout.Button(prefabName))
                    OpenPrefab(prefabPath);
            }
        }
        else
        {
            GUILayout.Label("No prefabs found in Assets/Prefabs");
        }
    }

    private void OpenPrefab(string prefabPath)
    {
        if (File.Exists(prefabPath))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                Selection.activeObject = prefab;
                Debug.Log($"Opened prefab: {Path.GetFileNameWithoutExtension(prefabPath)}");
            }
            else
            {
                Debug.LogWarning($"Prefab not found: {prefabPath}");
            }
        }
        else
        {
            Debug.LogWarning($"Prefab file not found: {prefabPath}");
        }
    }

    private void OnInspectorUpdate()
    {
        if (EditorApplication.timeSinceStartup - lastUpdateTime <= 2)
            return;

        LoadPrefabs();
        lastUpdateTime = EditorApplication.timeSinceStartup;
        Repaint();
    }
}
