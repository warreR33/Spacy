using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Shield")]
    public bool shieldActive = true;
    public float shieldCharge = 100f;
    public float maxShieldCharge = 100f;
    public float shieldRechargeRate = 10f;
    public bool useShield;

    private BaseShield customShield;

    [Header("Optional Callbacks")]
    public Action<int, int> OnHealthChanged;
    public Action<float, float, bool> OnShieldChanged;
    public Action OnDeath;

    public bool IsDead { get; private set; } = false;

    public bool initialized = false;

    private void Start()
    {

        if (!useShield)
        {
            shieldActive = false;
            shieldCharge = 0f;
        }

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnShieldChanged?.Invoke(shieldCharge, maxShieldCharge, shieldActive);
        initialized = true;
    }

    private void Update()
    {
        RechargeShield();
        customShield?.UpdateShield();
    }

    public void AttachShield(BaseShield shield)
    {
        customShield = shield;
        customShield.ApplyTo(this);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        if (shieldActive && useShield)
        {
            shieldActive = false;
            shieldCharge = 0f;
            OnShieldChanged?.Invoke(shieldCharge, maxShieldCharge, shieldActive);
            customShield?.OnShieldBroken();
            customShield?.OnDamageBlocked();
            return;
        }

        customShield?.OnDamageTakenWhileShieldDown();

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void RechargeShield()
    {
        if (!useShield || shieldActive || IsDead) return;

        shieldCharge += shieldRechargeRate * Time.deltaTime;

        if (shieldCharge >= maxShieldCharge)
        {
            shieldCharge = maxShieldCharge;
            shieldActive = true;
        }

        OnShieldChanged?.Invoke(shieldCharge, maxShieldCharge, shieldActive);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetShieldRechargeRate(float newRate)
    {
        shieldRechargeRate = newRate;
    }

    public void SetMaxShieldCharge(float newMax)
    {
        maxShieldCharge = newMax;
        shieldCharge = Mathf.Min(shieldCharge, maxShieldCharge);
        OnShieldChanged?.Invoke(shieldCharge, maxShieldCharge, shieldActive);
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
    }
}
