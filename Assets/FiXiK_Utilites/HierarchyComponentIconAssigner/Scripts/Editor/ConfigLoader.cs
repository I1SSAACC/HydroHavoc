#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    public static class ConfigLoader
    {
        public const string ConfigPath = "Assets/FiXiK_Utilites/HierarchyComponentIconAssigner/Config.asset";
        private const string RootFolder = "Assets";

        private static HierarchyIconSettings _config;

        public static HierarchyIconSettings Config => Load();

        public static HierarchyIconSettings Load()
        {
            if (_config != null)
                return _config;

            _config = AssetDatabase.LoadAssetAtPath<HierarchyIconSettings>(ConfigPath);

            if (_config != null)
                return _config;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(HierarchyIconSettings)}", new[] { RootFolder });

            if (guids.Length == 0)
            {
                return CreateConfig();
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            _config = AssetDatabase.LoadAssetAtPath<HierarchyIconSettings>(assetPath);

            if (guids.Length > 1)
                Debug.LogWarning($"Найдено несколько файлов настроек. Используется первый: {assetPath}");

            return _config;
        }

        private static HierarchyIconSettings CreateConfig()
        {
            string settingsPath = ConfigPath;
            string directory = Path.GetDirectoryName(settingsPath);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }

            HierarchyIconSettings instance = ScriptableObject.CreateInstance<HierarchyIconSettings>();
            AssetDatabase.CreateAsset(instance, settingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _config = AssetDatabase.LoadAssetAtPath<HierarchyIconSettings>(settingsPath);

            if (_config == null)
                Debug.LogError($"Не удалось создать файл настроек {nameof(HierarchyIconSettings)} {nameof(_config)}");

            return _config;
        }
    }
}
#endif