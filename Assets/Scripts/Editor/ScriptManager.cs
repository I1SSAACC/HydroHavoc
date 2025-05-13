using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ScriptManager : EditorWindow
{
    private List<FolderInfo> folders;
    private Dictionary<string, bool> folderStates = new Dictionary<string, bool>();

    [MenuItem("Tools/Script Manager")]
    public static void ShowWindow() => GetWindow<ScriptManager>("Script Manager");

    private void OnEnable()
    {
        LoadScripts();
    }

    private void LoadScripts()
    {
        string scriptsFolder = "Assets/Scripts";
        folders = new List<FolderInfo>();
        if (Directory.Exists(scriptsFolder))
            LoadFolder(scriptsFolder);
        else
            Debug.LogWarning($"Directory not found: {scriptsFolder}");
    }

    private void LoadFolder(string folderPath)
    {
        string[] directories = Directory.GetDirectories(folderPath);
        string[] scripts = Directory.GetFiles(folderPath, "*.cs");

        FolderInfo folderInfo = new FolderInfo
        {
            Name = Path.GetFileName(folderPath),
            Scripts = scripts,
            SubFolders = new List<FolderInfo>()
        };

        foreach (string directory in directories)
            LoadFolder(directory);

        folders.Add(folderInfo);
    }

    private void OnGUI()
    {
        if (folders != null && folders.Count > 0)
        {
            foreach (var folder in folders)
                DrawFolder(folder);
        }
        else
        {
            GUILayout.Label("No folders found in Assets/Scripts");
        }
    }

    private void DrawFolder(FolderInfo folder)
    {
        bool isOpen = folderStates.ContainsKey(folder.Name) && folderStates[folder.Name];

        if (GUILayout.Button(folder.Name, EditorStyles.boldLabel))
            folderStates[folder.Name] = !isOpen;

        if (isOpen)
        {
            if (folder.Scripts.Length > 0)
            {
                foreach (var scriptPath in folder.Scripts)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(scriptPath);
                    if (GUILayout.Button("  " + scriptName))
                        OpenScript(scriptPath);
                }
            }
            else
            {
                GUILayout.Label("  No scripts found in this folder");
            }

            foreach (var subFolder in folder.SubFolders)
                DrawFolder(subFolder);

            GUILayout.Space(5);
        }
    }

    private void OpenScript(string scriptPath)
    {
        UnityEditor.AssetDatabase.OpenAsset(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath));
    }

    private class FolderInfo
    {
        public string Name { get; set; }
        public string[] Scripts { get; set; }
        public List<FolderInfo> SubFolders { get; set; }
    }
}