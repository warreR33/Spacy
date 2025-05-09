using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentHUD : MonoBehaviour
{
    public Button weapon1Button;
    public Button weapon2Button;

    public ShipEquipmentController equipmentController;

    public GameObject weapon1Prefab;
    public GameObject weapon2Prefab;

    public TutorialManager tutorialManager;

    private void Start()
    {
        weapon1Button.onClick.AddListener(() => EquipWeapon(weapon1Prefab));
        weapon2Button.onClick.AddListener(() => EquipWeapon(weapon2Prefab));
    }

    void EquipWeapon(GameObject weaponPrefab)
    {
        if (equipmentController == null || weaponPrefab == null)return;
       

        GameObject weaponInstance = Instantiate(weaponPrefab, equipmentController.transform);
        BaseWeapon newWeapon = weaponInstance.GetComponent<BaseWeapon>();

        if (equipmentController.EquipWeapon(newWeapon))
        {
            tutorialManager?.OnWeaponEquipped(); 
        }
    }
}
