using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    OnLevel,
    InTransition
}

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;
    [SerializeField]LevelTransitionVisual transition;

    public GameState currentState = GameState.OnLevel;

    public int currentLevel = 0;
    public int pointsPerLevel = 500;

    public delegate void LevelChanged(int newLevel);
    public event LevelChanged OnLevelChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void CheckProgress(int totalPoints)
    {
        int newLevel = totalPoints / pointsPerLevel;
        if (newLevel > currentLevel && currentState == GameState.OnLevel)
        {
            currentLevel = newLevel;
            CompleteTransition(); 
        }
    }

    public void StartTransition()
    {
        currentState = GameState.InTransition;

        transition.gameObject.SetActive(true);
        if (transition != null){
            transition.StartTransition();}
            
        else
            Debug.LogWarning("No se encontró un LevelTransitionVisual en la escena.");
    }

    public void CompleteTransition()
    {
        currentState = GameState.OnLevel;
        OnLevelChanged?.Invoke(currentLevel);
    }
}
