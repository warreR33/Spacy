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
    private bool isTouching = false;

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
        inputActions.Gameplay.TouchPosition.started += OnTouchStart;
        inputActions.Gameplay.TouchPosition.performed += OnTouchMove;
        inputActions.Gameplay.TouchPosition.canceled += OnTouchEnd;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.TouchPosition.started -= OnTouchStart;
        inputActions.Gameplay.TouchPosition.performed -= OnTouchMove;
        inputActions.Gameplay.TouchPosition.canceled -= OnTouchEnd;
        inputActions.Gameplay.Disable();
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isTouching = true;
    }

    private void OnTouchMove(InputAction.CallbackContext context)
    {
        if (isTouching)
        {
            Vector2 screenPosition = context.ReadValue<Vector2>();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
            worldPos.z = 0;
            targetPosition = worldPos;
        }
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        isTouching = false;
        targetPosition = rb.position;
    }

    private void FixedUpdate()
    {
        Vector2 inputDir = inputActions.Gameplay.Move.ReadValue<Vector2>();
        
        Vector2 movement = inputDir.normalized * moveSpeed;
        rb.velocity = movement;

        if (inputDir != Vector2.zero)
        {
            targetPosition = rb.position + inputDir * moveSpeed * Time.fixedDeltaTime;
        }

        Vector2 newPos = Vector2.Lerp(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        foreach (BaseWeapon weapon in weaponSlots)
        {
            if (weapon != null)
                weapon.UpdateWeapon();
        }
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
