using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEngine : MonoBehaviour
{
    public int bonusHealth = 0;

    public virtual void ApplyTo(HealthSystem healthSystem)
    {
        int newMaxHealth = healthSystem.maxHealth + bonusHealth;
        healthSystem.SetMaxHealth(newMaxHealth);
    }
}
