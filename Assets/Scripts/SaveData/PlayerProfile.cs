using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public static class PlayerProfile
{
    private const string PlayerNameKey = "PlayerName";

    public static string PlayerName
    {
        get => PlayerPrefs.GetString(PlayerNameKey, null);
        set
        {
            PlayerPrefs.SetString(PlayerNameKey, value);
            PlayerPrefs.Save();
        }
    }

    public static bool HasProfile => !string.IsNullOrEmpty(PlayerName);
}

public class NameInputPanel : MonoBehaviour
{
    public TMP_InputField nameField;

    public void OnConfirmName()
    {
        string name = nameField.text.Trim();
        if (!string.IsNullOrEmpty(name))
        {
            PlayerProfile.PlayerName = name;
            SceneLoader.LoadScene("MainMenu");
        }
    }
}
