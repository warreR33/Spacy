using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShield : MonoBehaviour
{
    public float bonusRechargeRate = 0f;
    public float bonusMaxCharge = 0f;

    protected HealthSystem healthSystem;

    public virtual void ApplyTo(HealthSystem hs)
    {
        healthSystem = hs;
        healthSystem.SetShieldRechargeRate(healthSystem.shieldRechargeRate + bonusRechargeRate);
        healthSystem.SetMaxShieldCharge(healthSystem.maxShieldCharge + bonusMaxCharge);
    }

    public virtual void UpdateShield() { }
    public virtual void OnShieldBroken() { }
    public virtual void OnDamageBlocked() { }
    public virtual void OnDamageTakenWhileShieldDown() { }
}
