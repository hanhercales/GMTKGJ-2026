using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private LayerMask interactableLayers = ~0;

    private InputAction clickAction;
    private InputAction pointAction;

    private IDragInputHandler currentDragTarget;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        pointAction = new InputAction("Point", InputActionType.Value, "<Mouse>/position");
        clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");

        clickAction.started += OnClickStarted;
        clickAction.canceled += OnClickCanceled;
    }

    private void OnEnable()
    {
        pointAction.Enable();
        clickAction.Enable();
    }
    
    private void OnDisable()
    {
        pointAction.Disable();
        clickAction.Disable();
    }

    private void Update()
    {
        if(currentDragTarget != null)
            currentDragTarget.OnDragUpdate(GetPointerWorldPos());
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Vector2 worldPos = GetPointerWorldPos();
        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactableLayers);
        if (hit == null) return;

        if (hit.TryGetComponent(out IDragInputHandler dragHandler))
        {
            currentDragTarget = dragHandler;
            dragHandler.OnDragStart(worldPos);
        }
        else if(hit.TryGetComponent(out IClickInputHandler clickHandler))
            clickHandler.OnClickDown(worldPos);
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        if (currentDragTarget == null) return;
        
        currentDragTarget.OnDragEnd(GetPointerWorldPos());
        currentDragTarget = null;
    }

    private Vector2 GetPointerWorldPos()
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
