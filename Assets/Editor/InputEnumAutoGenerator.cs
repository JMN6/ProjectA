#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Text;

public class InputEnumAutoGenerator : AssetPostprocessor
{
    private const string outputPath = "Assets/InputActions/InputActionsEnum.cs";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] _, string[] __, string[] ___)
    {
        foreach (string path in importedAssets)
        {
            if (path.EndsWith(".inputactions"))
            {
                GenerateEnum(path);
            }
        }
    }

    private static void GenerateEnum(string assetPath)
    {
        var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);
        if (inputAsset == null)
            return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("namespace Game.Input");
        sb.AppendLine("{");
        sb.AppendLine("    public enum Maps");
        sb.AppendLine("    {");

        foreach (var map in inputAsset.actionMaps)
        {
            string sanitized = map.name;
            sb.AppendLine($"        {sanitized},");
        }

        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    public enum Actions");
        sb.AppendLine("    {");

        foreach (var map in inputAsset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                string sanitizedAction = action.name;
                sb.AppendLine($"        {sanitizedAction},");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        // 폴더가 없으면 생성
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));

        File.WriteAllText(outputPath, sb.ToString());
        Debug.Log($"Inputs enum regenerated from {assetPath}");

        AssetDatabase.Refresh();
    }
}
#endif
