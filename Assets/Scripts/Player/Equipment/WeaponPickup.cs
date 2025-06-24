using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public BaseWeapon weaponPrefab;
    public float lifetime = 10f;
    
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 30f;

    private Vector3 startPos;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        startPos = transform.position;  
    }
    
    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ShipEquipmentController equipment = other.GetComponent<ShipEquipmentController>();
        if (equipment != null && weaponPrefab != null)
        {
            BaseWeapon weaponInstance = Instantiate(weaponPrefab);
            bool equipped = equipment.EquipWeapon(weaponInstance);

            if (equipped)
            {
                // ✅ Notificar al tutorial si estamos en el modo Tutorial
                if (GameProgressManager.Instance != null && GameProgressManager.Instance.currentState == GameState.Tutorial)
                {
                    TutorialManager tutorial = FindObjectOfType<TutorialManager>();
                    tutorial?.OnWeaponEquipped();
                }

                StartCoroutine(RemoveAfterTime(equipment, weaponInstance));
                Destroy(gameObject);
            }
            else
            {
                Destroy(weaponInstance.gameObject);
            }
        }
    }

    private System.Collections.IEnumerator RemoveAfterTime(ShipEquipmentController equipment, BaseWeapon weapon)
    {
        yield return new WaitForSeconds(8f); 
        equipment.UnequipWeapon(weapon);
    }
}
