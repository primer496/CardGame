using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 独立协程载体。创建普通 GameObject（非 DontDestroyOnLoad），
/// 在旧场景中运行协程，LoadScene 时随旧场景一起自动销毁，不留残留。
/// </summary>
public class SceneLoadHelper : MonoBehaviour
{
    public static void LoadDelayed(int buildIndex)
    {
        var go = new GameObject("_SceneLoadHelper");
        go.AddComponent<SceneLoadHelper>().StartCoroutine(DoLoad(buildIndex));
    }

    private static IEnumerator DoLoad(int buildIndex)
    {
        yield return null;
        SceneManager.LoadScene(buildIndex);
    }
}
