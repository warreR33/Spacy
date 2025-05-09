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

        tutorialText.text = "¡Pick a weapon!";
        yield return FadeInText();

        weaponButtonsGroup.SetActive(true); 
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
        StartCoroutine(FadeOutText()); 
        
        GameProgressManager.Instance.CompleteTutorial();
    }
}
