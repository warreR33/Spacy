using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{

    public HealthSystem healthSystem;

    [Header("References")]
    public PlayerHUDController hud;
    public PlayerAudioController audioController;
    public ShipMovement movementScript;
    public ShipEquipmentController shipEquipment;
    public GameObject defeatHUD;

    public static event System.Action OnPlayerDeath;

    private int lastHealth = -1;

    private void Start()
    {
        healthSystem.OnHealthChanged += UpdateHealth;
        healthSystem.OnShieldChanged += UpdateShield;
        healthSystem.OnDeath += HandleDeath;
    }


    private void UpdateHealth(int current, int max)
    {
        hud?.UpdateHealthSprite(current, max);
        hud?.UpdateHealthUI(current, max);

        if (healthSystem.initialized && lastHealth != -1 && current < lastHealth)
        {
            audioController?.PlayDamageSound();
        }

        lastHealth = current;
    }

    private void UpdateShield(float current, float max, bool isActive)
    {
        hud?.UpdateShield(current, max, isActive);
    }

    private void HandleDeath()
    {
        OnPlayerDeath?.Invoke();
        movementScript.enabled = false;
        StartCoroutine(audioController.PlayDeathSequence(() =>
        {
            StartCoroutine(DeathRoutine());
        }));
    }

    IEnumerator DeathRoutine()
    {
        Color originalColor = hud.spriteRenderer.color;
        for (int i = 0; i < 3; i++)
        {
            hud.spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            hud.spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        int score = Points.Instance.score;
        GameStats.LastScore = score;

        var saveTask = CloudSaveSystem.SaveScoreAsync(score);
        while (!saveTask.IsCompleted)
            yield return null;
            
        SceneLoader.LoadScene("Defeat");
    }

    IEnumerator SaveHighScoreIfNeeded()
    {
        var task = CloudSaveSystem.SaveScoreAsync(Points.Instance.score);
        while (!task.IsCompleted)
            yield return null;
    }

    IEnumerator ShowDefeatHUD()
    {
        int score = Points.Instance.score;
        
        var saveTask = CloudSaveSystem.SaveScoreAsync(score);
        while (!saveTask.IsCompleted)
            yield return null;

        var loadTask = CloudSaveSystem.LoadHighScoreAsync();
        while (!loadTask.IsCompleted)
            yield return null;

        int highScore = loadTask.Result;
    }
}
