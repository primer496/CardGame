using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class JsonToSOImporter
{
    private const string AssetDir = "Assets/Resources/Config";

    [MenuItem("CardGame/Update Config from Excel")]
    public static void UpdateConfigFromExcel()
    {
        // Step 1: xlsx -> JSON via Python script
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string scriptPath  = Path.Combine(projectRoot, "xlsx_to_json.py");

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("[ConfigImport] xlsx_to_json.py not found: " + scriptPath);
            return;
        }

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName               = "python",
                Arguments              = $"\"{scriptPath}\"",
                WorkingDirectory       = projectRoot,
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding  = Encoding.UTF8,
                CreateNoWindow         = true,
            }
        };

        proc.Start();
        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            UnityEngine.Debug.LogError("[ConfigImport] Python script failed:\n" + stderr);
            return;
        }
        if (!string.IsNullOrEmpty(stdout))
            UnityEngine.Debug.Log("[ConfigImport] Python:\n" + stdout);

        // Step 2: JSON -> SO
        bool anyDone = false;
        anyDone |= Import<AudioConfigSO>  ("AudioConfig");
        anyDone |= Import<DeckConfigSO>   ("DeckConfig");
        anyDone |= Import<CardGameRuleSO> ("CardGameRule");

        if (anyDone)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("[ConfigImport] Done. All SOs updated from Excel.");
        }
    }

    private static bool Import<T>(string name) where T : ScriptableObject
    {
        string jsonAbsolute = Path.Combine(Application.dataPath, "Resources", "Config", $"{name}.json");
        string assetPath    = $"{AssetDir}/{name}.asset";

        if (!File.Exists(jsonAbsolute))
        {
            UnityEngine.Debug.LogWarning($"[ConfigImport] JSON not found: {jsonAbsolute}");
            return false;
        }

        var so = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (so == null)
        {
            UnityEngine.Debug.LogWarning($"[ConfigImport] SO asset not found: {assetPath}. Run CardGame > Create Config Assets first.");
            return false;
        }

        string json = File.ReadAllText(jsonAbsolute, Encoding.UTF8);
        JsonUtility.FromJsonOverwrite(json, so);
        EditorUtility.SetDirty(so);
        UnityEngine.Debug.Log($"[ConfigImport] {name}.asset updated.");
        return true;
    }
}
