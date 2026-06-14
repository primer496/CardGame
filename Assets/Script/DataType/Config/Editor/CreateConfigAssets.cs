using UnityEditor;
using UnityEngine;

/// <summary>
/// 一键生成 Resources/Config/ 下的三个游戏配置 ScriptableObject 资源文件。
/// 菜单路径：CardGame -> Create Config Assets
/// </summary>
public static class CreateConfigAssets
{
    [MenuItem("CardGame/Create Config Assets")]
    public static void Create()
    {
        const string dir = "Assets/Resources/Config";
        if (!AssetDatabase.IsValidFolder(dir))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Config");
        }

        CreateIfMissing<AudioConfigSO>(dir, "AudioConfig");
        CreateIfMissing<DeckConfigSO>(dir,  "DeckConfig");
        CreateIfMissing<CardGameRuleSO>(dir, "CardGameRule");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[CreateConfigAssets] 配置资产已生成于 " + dir);
    }

    private static void CreateIfMissing<T>(string dir, string name) where T : ScriptableObject
    {
        string path = $"{dir}/{name}.asset";
        if (AssetDatabase.LoadAssetAtPath<T>(path) != null)
        {
            Debug.Log($"[CreateConfigAssets] 已存在，跳过：{path}");
            return;
        }
        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        Debug.Log($"[CreateConfigAssets] 已创建：{path}");
    }
}
