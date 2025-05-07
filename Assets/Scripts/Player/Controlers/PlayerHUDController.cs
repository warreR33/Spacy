using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHUDController : MonoBehaviour
{

    [Header("HUD HP")]
    public GameObject healthContainer;
    public Sprite heartFull;
    public Sprite heartEmpty;

    [Header("Ship Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite fullHealthSprite;
    public Sprite highHealthSprite;
    public Sprite lowHealthSprite;
    public Sprite criticalHealthSprite;

    [Header("Shield Visuals")]
    public Image shieldHUD;
    public SpriteRenderer shieldVisual;

    [Header("Ammo UI")]
    public TextMeshProUGUI ammoText;
    public Image reloadProgressCircle; 

    private List<Image> hearts = new List<Image>();

    void Awake()
    {
        foreach (Transform child in healthContainer.transform)
        {
            Image heart = child.GetComponent<Image>();
            if (heart != null) hearts.Add(heart);
        }
    }

    public void UpdateHealthUI(int current, int max)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < current ? heartFull : heartEmpty;
        }
    }

    public void UpdateHealthSprite(int current, int max)
    {
        float percent = (float)current / max;
        if (percent > 0.75f) spriteRenderer.sprite = fullHealthSprite;
        else if (percent > 0.5f) spriteRenderer.sprite = highHealthSprite;
        else if (percent > 0.25f) spriteRenderer.sprite = lowHealthSprite;
        else spriteRenderer.sprite = criticalHealthSprite;
    }

    public void UpdateShield(float charge, float max, bool active)
    {
        if (shieldHUD != null)
        {
            float percent = Mathf.Clamp01(charge / max);

            if (active)
                shieldHUD.fillAmount = 1f;
            else
                shieldHUD.fillAmount = percent;
        }

        if (shieldVisual != null)
            shieldVisual.enabled = active;
            
    }

    public void UpdateAmmoUI(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{current}";
        }
    }

    public void UpdateReloadUI(float timePassed, float totalTime)
    {
        if (reloadProgressCircle != null)
        {
            float fillAmount = Mathf.Clamp01(timePassed / totalTime);
            reloadProgressCircle.fillAmount = fillAmount;
        }
    }

    public void ResetReloadUI()
    {
        if (reloadProgressCircle != null)
        {
            reloadProgressCircle.fillAmount = 0f;
        }
    }

}
