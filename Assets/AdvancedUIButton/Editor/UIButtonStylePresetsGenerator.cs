#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AdvancedUI.Editor
{
    /// <summary>
    /// Generates the 5 built-in style preset .asset files in the project.
    /// Access via Tools → AdvancedUI → Generate Style Presets.
    /// </summary>
    public static class UIButtonStylePresetsGenerator
    {
        private const string OutputPath = "Assets/AdvancedUIButton/Presets";

        [MenuItem("Tools/AdvancedUI/Generate Style Presets", priority = 1)]
        public static void GenerateAll()
        {
            EnsureDirectory(OutputPath);

            CreateAsset(UIButtonStylePresets.Primary(),   "Style_Primary");
            CreateAsset(UIButtonStylePresets.Secondary(), "Style_Secondary");
            CreateAsset(UIButtonStylePresets.Danger(),    "Style_Danger");
            CreateAsset(UIButtonStylePresets.Outline(),   "Style_Outline");
            CreateAsset(UIButtonStylePresets.Ghost(),     "Style_Ghost");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "AdvancedUI",
                $"5 style presets generated in:\n{OutputPath}",
                "OK"
            );

            // Ping the folder in the Project panel
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(OutputPath);
            if (folder != null) EditorGUIUtility.PingObject(folder);
        }

        private static void CreateAsset(UIButtonStyle style, string assetName)
        {
            string path = $"{OutputPath}/{assetName}.asset";

            // Overwrite if exists so re-generating is safe
            if (File.Exists(Path.GetFullPath(path)))
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateAsset(style, path);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(
                    Path.GetDirectoryName(path),
                    Path.GetFileName(path)
                );
        }
    }
}
#endif
