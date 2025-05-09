using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShield : MonoBehaviour
{
    public float bonusRechargeRate = 0f;
    public float bonusMaxCharge = 0f;

    public virtual void ApplyTo(HealthSystem healthSystem)
    {
        healthSystem.SetShieldRechargeRate(healthSystem.shieldRechargeRate + bonusRechargeRate);
        healthSystem.SetMaxShieldCharge(healthSystem.maxShieldCharge + bonusMaxCharge);
    }
}
