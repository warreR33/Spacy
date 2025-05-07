using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
     public GameObject projectilePrefab;
    public int damage;
    public float cooldown = 0.5f;
    public Transform[] shootPoints;

    private PlayerHUDController hud;

    [Header("Ammo System")]
    public int maxAmmo = 10;
    public float reloadTime = 2f;

    private int currentAmmo;
    private float timer = 0f;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    private int currentShootIndex = 0;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        hud = FindObjectOfType<PlayerHUDController>();
        hud?.UpdateAmmoUI(currentAmmo, maxAmmo);
        hud?.ResetReloadUI();
    }

    public void UpdateWeapon()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            hud?.UpdateReloadUI(reloadTimer, reloadTime);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                timer = 0f;

                hud?.UpdateAmmoUI(currentAmmo, maxAmmo);

                if (currentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                }
            }
        }
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
        if (shootPoints == null || shootPoints.Length == 0) return null;

        Transform point = shootPoints[currentShootIndex];
        currentShootIndex = (currentShootIndex + 1) % shootPoints.Length;
        return point;
    }

    // 🔧 Opción para animación
    protected void SetShootingAnimation(bool state)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("isShooting", state);
    }

    public abstract void Shoot();
}
