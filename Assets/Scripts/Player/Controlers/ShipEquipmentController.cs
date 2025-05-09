using System.Collections.Generic;
using UnityEngine;

public class ShipEquipmentController : MonoBehaviour
{
    public List<BaseWeapon> equippedWeapons = new List<BaseWeapon>();
    public List<BaseShield> equippedShields = new List<BaseShield>();
    public List<BaseEngine> equippedEngines = new List<BaseEngine>();

    public int maxWeapons = 2;
    public int maxShields = 1;
    public int maxEngines = 1;

    public bool EquipWeapon(BaseWeapon newWeapon)
    {
        if (newWeapon == null)
            {
                Debug.LogWarning("No se encontró componente BaseWeapon en el prefab instanciado.");
                return false;
            }

            if (equippedWeapons.Count >= maxWeapons)
            {
                Debug.Log("Ya se alcanzó el máximo de armas.");
                return false;
            }

            equippedWeapons.Add(newWeapon);
            newWeapon.transform.SetParent(transform);
            newWeapon.transform.localPosition = Vector3.zero;

            Debug.Log("Arma equipada correctamente: " + newWeapon.name);
            Debug.Log("Cantidad total de armas: " + equippedWeapons.Count);
            return true;
    }

    public void UnequipWeapon(BaseWeapon weapon)
    {
        if (equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Remove(weapon);
            Destroy(weapon.gameObject);
        }
    }

    public bool EquipShield(BaseShield newShield)
    {
        if (equippedShields.Count >= maxShields)
            return false;

        equippedShields.Add(newShield);
        newShield.transform.SetParent(transform);
        newShield.transform.localPosition = Vector3.zero;
        return true;
    }

    public void UnequipShield(BaseShield shield)
    {
        if (equippedShields.Contains(shield))
        {
            equippedShields.Remove(shield);
            Destroy(shield.gameObject);
        }
    }

    public bool EquipEngine(BaseEngine newEngine)
    {
        if (equippedEngines.Count >= maxEngines)
            return false;

        equippedEngines.Add(newEngine);
        newEngine.transform.SetParent(transform);
        newEngine.transform.localPosition = Vector3.zero;
        return true;
    }

    public void UnequipEngine(BaseEngine engine)
    {
        if (equippedEngines.Contains(engine))
        {
            equippedEngines.Remove(engine);
            Destroy(engine.gameObject);
        }
    }
}
