using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int damage;
    public Transform[] shootPoints;

    private PlayerHUDController hud;

    [Header("Ammo System")]
    public int maxAmmo = 10;
    public float reloadTime = 2f;

    private int currentAmmo;
    private float timer = 0f;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    protected int currentShootIndex = 0;
    private bool readyToShoot = false;
    public bool fireAllAtOnce = false;
    public bool autoFire = true;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        hud = FindObjectOfType<PlayerHUDController>();
        hud?.UpdateAmmoUI(currentAmmo, maxAmmo);
        hud?.ResetReloadUI();
    }
  

    protected virtual void Update()
    {
        if (autoFire)
            UpdateWeapon();
    }
    
    public void UpdateWeapon()
    {
        if (isReloading) {
            reloadTimer += Time.deltaTime;
            hud?.UpdateReloadUI(reloadTimer, reloadTime);
            return;
        }

        timer += Time.deltaTime;

        if (currentAmmo > 0)
        {
            TriggerShoot();
            timer = 0f;
        }
    }

    private void TriggerShoot()
    {
        readyToShoot = true;
        SetShootingAnimation(true);
    }


    private IEnumerator Reload()
    {
        isReloading = true;
        SetShootingAnimation(false);
        reloadTimer = 0f;
        hud?.ResetReloadUI();

        while (reloadTimer < reloadTime)
        {
            yield return null;
            reloadTimer += Time.deltaTime;
            hud?.UpdateReloadUI(reloadTimer, reloadTime);
        }

        currentAmmo = maxAmmo;
        isReloading = false;
        hud?.UpdateAmmoUI(currentAmmo, maxAmmo);
        hud?.ResetReloadUI();
        SetShootingAnimation(true);
    }

    protected Transform GetNextShootPoint()
    {
        if (shootPoints == null || shootPoints.Length == 0){
        Debug.LogWarning("No shoot points assigned.");
        return null;
        }
        Transform point = shootPoints[currentShootIndex];
        currentShootIndex = (currentShootIndex + 1) % shootPoints.Length;
        return point;
    }

    protected void SetShootingAnimation(bool state)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("isShooting", state);
    }
    
    public void FireFromAnimation()
    {
        if (!readyToShoot || isReloading || currentAmmo <= 0) return;

        if (fireAllAtOnce)
        {
            for (int i = 0; i < shootPoints.Length && currentAmmo > 0; i++)
            {
                Shoot();
                currentAmmo--;
            }
        }
        else
        {
            Shoot();
            currentAmmo--;
        }

        currentAmmo = Mathf.Max(0, currentAmmo);
        hud?.UpdateAmmoUI(currentAmmo, maxAmmo);
        readyToShoot = false;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        SetShootingAnimation(false);
    }
    public abstract void Shoot();
}
