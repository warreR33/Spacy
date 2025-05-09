using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{

    public static event System.Action OnShipMoved;
    private Vector2 previousPosition;


    [SerializeField] private float moveSpeed = 10f;
    private Vector3 targetPosition;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Vector2 dragDirection;
    
    private PlayerInputActions inputActions;
    private bool isTouching = false;

    private Vector2 lastTouchPosition;
    private float dragThreshold = 15f;
    private float dragSensitivity = 0.05f;

    [SerializeField] private Transform startPosition;


    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        if (startPosition != null)
        {
            transform.position = startPosition.position;
            targetPosition = startPosition.position;
        }
        else
        {
            targetPosition = transform.position;
        }
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
        lastTouchPosition = context.ReadValue<Vector2>();
    }

    private void OnTouchMove(InputAction.CallbackContext context)
    {
        if (!isTouching) return;

        Vector2 currentTouch = context.ReadValue<Vector2>();
        Vector2 delta = currentTouch - lastTouchPosition;

        if (delta.magnitude > dragThreshold)
        {
            dragDirection = delta * dragSensitivity;
            lastTouchPosition = currentTouch;
        }
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        isTouching = false;
        dragDirection = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector2 inputDir = inputActions.Gameplay.Move.ReadValue<Vector2>();
        Vector2 movement;

        // Movimiento con teclas
        if (inputDir != Vector2.zero)
        {
            movement = inputDir.normalized * moveSpeed;
            dragDirection = Vector2.zero; 
        }
        // Movimiento por arrastre
        else if (isTouching && dragDirection != Vector2.zero)
        {
            movement = dragDirection * moveSpeed;
        }
        else
        {
            movement = Vector2.zero;
        }

        // Suavizado de movimiento (si hay algún movimiento)
        Vector2 targetVelocity = movement;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * 10f);

        // Verificar si la nave se movió
        if ((Vector2)transform.position != previousPosition)
        {
            previousPosition = transform.position;
            OnShipMoved?.Invoke();
        }

    }
}
