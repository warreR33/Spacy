using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public static event System.Action OnShipMoved;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dragThreshold = 10f;
    [SerializeField] private float dragSensitivity = 0.05f;
    [SerializeField] private Transform startPosition;

    private Vector2 previousPosition;
    private Vector2 lastTouchPosition;
    private Vector2 dragDelta;
    private bool isDragging = false;
    private bool hasMoved = false;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private PlayerInputActions inputActions;

    private Vector2 keyboardInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        if (startPosition != null)
        {
            transform.position = startPosition.position;
        }
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.TouchPress.started += OnTouchStart;
        inputActions.Gameplay.TouchPress.canceled += OnTouchEnd;
        inputActions.Gameplay.TouchPosition.performed += OnTouchMove;

        inputActions.Gameplay.Move.performed += OnMovePerformed;
        inputActions.Gameplay.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.TouchPress.started -= OnTouchStart;
        inputActions.Gameplay.TouchPress.canceled -= OnTouchEnd;
        inputActions.Gameplay.TouchPosition.performed -= OnTouchMove;

        inputActions.Gameplay.Move.performed -= OnMovePerformed;
        inputActions.Gameplay.Move.canceled -= OnMoveCanceled;

        inputActions.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        keyboardInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        keyboardInput = Vector2.zero;
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isDragging = true;
        lastTouchPosition = inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
    }

    private void OnTouchMove(InputAction.CallbackContext context)
    {
        if (!isDragging) return;

        Vector2 currentTouchPosition = context.ReadValue<Vector2>();
        Vector2 delta = currentTouchPosition - lastTouchPosition;

        if (delta.magnitude > dragThreshold)
        {
            dragDelta = delta;
        }
        else
        {
            dragDelta = Vector2.zero;
        }

        lastTouchPosition = currentTouchPosition;
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        isDragging = false;
        dragDelta = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector2 movement = Vector2.zero;

        if (keyboardInput != Vector2.zero)
        {
            movement = keyboardInput.normalized * moveSpeed;
        }
        else if (isDragging && dragDelta != Vector2.zero)
        {
            movement = dragDelta * dragSensitivity;
        }

        rb.velocity = Vector2.Lerp(rb.velocity, movement, Time.fixedDeltaTime * moveSpeed);

        if (!hasMoved && (keyboardInput != Vector2.zero || dragDelta != Vector2.zero))
        {
            hasMoved = true; // nueva variable local en ShipMovement
            OnShipMoved?.Invoke();
        }

        dragDelta = Vector2.zero;
    }
}
