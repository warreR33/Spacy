using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static string targetScene;

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;

        SceneManager.LoadScene("Loading");
    }

    public static void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("No se definió ninguna escena destino.");
        }
    }
}
