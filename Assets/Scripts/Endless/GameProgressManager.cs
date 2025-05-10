using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    OnLevel,
    InTransition,
    Tutorial 
}

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;
    [SerializeField]LevelTransitionVisual transition;
    [SerializeField] EnemySpawner enemySpawner;

    public GameState currentState = GameState.OnLevel;

    public int currentLevel = -1;
    public int pointsPerLevel = 500;

    public delegate void LevelChanged(int newLevel);
    public event LevelChanged OnLevelChanged;
    

    public delegate void SimpleCallback();
    public event SimpleCallback OnStartTransition;
    public event SimpleCallback OnCompleteTransition;

    public bool isInTutorial = true;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void CheckProgress(int totalPoints)
    {
        if (isInTutorial) return; 

        int newLevel = totalPoints / pointsPerLevel;
        if (newLevel > currentLevel && currentState == GameState.OnLevel)
        {
            currentLevel = newLevel;
            StartTransition();
            Debug.Log("Cambio de level");
        }
    }

    public void StartTransition()
    {
        currentState = GameState.InTransition;

        OnStartTransition?.Invoke(); 

        if (transition != null)
        {
            transition.gameObject.SetActive(true);
            transition.StartTransition();
        }
        else
        {
            Debug.LogWarning("No se encontró un LevelTransitionVisual en la escena.");
        }
    }

    public void CompleteTransition()
    {
        currentState = GameState.OnLevel;

        OnCompleteTransition?.Invoke(); 
        OnLevelChanged?.Invoke(currentLevel);
    }

    public void StartTutorial()
    {
        currentState = GameState.Tutorial;
        isInTutorial = true;
        if (enemySpawner != null)
        {
            StopCoroutine(enemySpawner.SpawnWaves()); 
        }
    }

    public void CompleteTutorial()
    {
        isInTutorial = false;
        currentState = GameState.OnLevel;
        currentLevel = 0; 

        if (enemySpawner != null)
        {
            StartCoroutine(enemySpawner.SpawnWaves()); 
        }

        OnCompleteTransition?.Invoke(); 
        //OnLevelChanged?.Invoke(currentLevel);
    }
}
