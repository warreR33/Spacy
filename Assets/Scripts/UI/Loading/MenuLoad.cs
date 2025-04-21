using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoad : MonoBehaviour
{

   public void LoadScene(string sceneName)
    {
        SceneLoader.LoadScene(sceneName);
    }

    

    public void ExitGame()
    {
        Application.Quit();
    }
}
