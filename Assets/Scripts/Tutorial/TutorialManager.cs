using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text tutorialText;
    public CanvasGroup tutorialCanvasGroup;
    public GameObject player;

    [Header("Tutorial Settings")]
    public float fadeDuration = 1f;

    [Header("Weapon Selection")]
    public GameObject weaponButtonsGroup; 
    public float waitAfterMove = 2f;
    public bool hasEquippedWeapon;

    [Header("Weapon Pickup System")]
    public List<GameObject> weaponPickupPrefabs; 
    public Transform weaponSpawnPoint; 

    public EnemySpawner enemySpawner;
    private bool hasMoved = false;
    private Coroutine blinkCoroutine;
    private SpriteRenderer playerSprite;

    private void OnEnable()
    {
        ShipMovement.OnShipMoved += HandlePlayerMoved;
    }

    private void OnDisable()
    {
        ShipMovement.OnShipMoved -= HandlePlayerMoved;
    }

    private void HandlePlayerMoved()
    {
        if (hasMoved) return;
        
        hasMoved = true;
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        SetPlayerAlpha(1f);
        StartCoroutine(FadeOutText());
        StartCoroutine(TransitionToWeaponSelection());
    }

    private void Start()
    {
        tutorialCanvasGroup.alpha = 1f;
        playerSprite = player.GetComponent<SpriteRenderer>();
        blinkCoroutine = StartCoroutine(BlinkPlayer());
        StartCoroutine(ShowMoveText());
    }

    private void Update()
    {
        if (!hasMoved && player.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            hasMoved = true;
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            SetPlayerAlpha(1f); 
            StartCoroutine(FadeOutText());

            StartCoroutine(TransitionToWeaponSelection());
        }
    }

    private IEnumerator ShowMoveText()
    {
        tutorialText.text = "¡Drag to move!";
        yield return null;
    }

    private IEnumerator TransitionToWeaponSelection()
    {
        yield return FadeOutText();
        yield return new WaitForSeconds(waitAfterMove);

        tutorialText.text = "¡Pick up the weapon!";
        yield return FadeInText();

        SpawnWeaponPickup();
    }

    private void SpawnWeaponPickup()
    {
        if (weaponPickupPrefabs.Count == 0 || weaponSpawnPoint == null)
        {
            Debug.LogWarning("No hay pickups o punto de spawn asignado.");
            return;
        }

        // Elegimos uno aleatorio
        int index = Random.Range(0, weaponPickupPrefabs.Count);
        GameObject selectedPickup = weaponPickupPrefabs[index];

        Instantiate(selectedPickup, weaponSpawnPoint.position, Quaternion.identity);
    }

    private IEnumerator FadeOutText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = 1f - (elapsedTime / fadeDuration);
            yield return null;
        }

        tutorialCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = (elapsedTime / fadeDuration);
            yield return null;
        }
        tutorialCanvasGroup.alpha = 1f;
    }

    private IEnumerator BlinkPlayer()
    {
        float blinkSpeed = 1.5f;
        float minAlpha = 0.2f;
        float maxAlpha = 1f;

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(t) + 1f) / 2f);
            SetPlayerAlpha(alpha);
            yield return null;
        }
    }

    private void SetPlayerAlpha(float alpha)
    {
        if (playerSprite != null)
        {
            Color color = playerSprite.color;
            color.a = alpha;
            playerSprite.color = color;
        }
    }

    public void OnWeaponEquipped()
    {
        if (hasEquippedWeapon) return;

        hasEquippedWeapon = true;
        weaponButtonsGroup.SetActive(false); 
        StartCoroutine(HandleEnemyTutorialPhase());
        FadeOutText();
        
    }

    private IEnumerator HandleEnemyTutorialPhase()
    {

        tutorialText.text = "¡Defeat all the ships!";
        yield return FadeInText();
        Debug.Log("Enemies");
        yield return FadeOutText();
        enemySpawner.OnWaveCleared += HandleWaveCleared;
        enemySpawner.SpawnSingleWave();
    }

    private void HandleWaveCleared()
    {
        enemySpawner.OnWaveCleared -= HandleWaveCleared;
        StartCoroutine(HandleTutorialComplete());
    }

    private IEnumerator HandleTutorialComplete()
    {
        tutorialText.text = "¡Well Done!";
        yield return FadeInText();
        
        yield return FadeOutText();

        GameProgressManager.Instance.CompleteTutorial();
    }
}
