using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private Vector3 targetPosition;
    private Camera mainCamera;
    private Rigidbody2D rb;
    
    private PlayerInputActions inputActions;

    public BaseWeapon[] weaponSlots = new BaseWeapon[2];
    
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.TouchPosition.performed += OnTouchPosition;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.TouchPosition.performed -= OnTouchPosition;
        inputActions.Gameplay.Disable();
    }

    private void OnTouchPosition(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        worldPos.z = 0;
        targetPosition = worldPos;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = Vector2.Lerp(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        
        foreach (BaseWeapon weapon in weaponSlots)
        {
            if (weapon != null)
                weapon.UpdateWeapon(); 
        }

    }

    private void Update()
    {

    }

    public bool EquipWeapon(BaseWeapon newWeapon)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = newWeapon;
                newWeapon.transform.SetParent(transform);
                newWeapon.transform.localPosition = Vector3.zero; 
                return true;
            }
        }
        return false;
    }
}
