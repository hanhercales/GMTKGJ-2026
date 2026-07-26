using System;
using UnityEngine;

[RequireComponent(typeof(PointerReceiver))]
[RequireComponent(typeof(Collider2D))]
public class DeskObjectFocus : MonoBehaviour
{
    [Header("Focus Settings")]
    [SerializeField] private Vector3 focusedPosition = Vector3.zero;
    [SerializeField] private Vector3 focusedScale = Vector3.one * 2f;
    [SerializeField] private int focusedSortingOrder = 100;
    [SerializeField] private Behaviour[] componentsToEnableOnFocus;
    
    private PointerReceiver receiver;
    private SpriteRenderer[] renderers;
    private Collider2D ownCollider;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private int[] originalSortingOrders;

    private bool isFocused;

    private void Awake()
    {
        receiver = GetComponent<PointerReceiver>();
        ownCollider = GetComponent<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        
        originalPosition = transform.position;
        originalScale = transform.localScale;
        
        originalSortingOrders = new int[renderers.Length];
        for(int i = 0; i < renderers.Length; i++)
            originalSortingOrders[i] = renderers[i].sortingOrder;
        
        SetInteractable(false);
    }
    
    private void OnEnable() => receiver.ClickDown += HandleClick;
    private void OnDisable() => receiver.ClickDown -= HandleClick;

    private void HandleClick(Vector2 worldPos)
    {
        if (!isFocused) Focus();
    }

    private void Focus()
    {
        if(isFocused)  return;
        isFocused = true;
        
        transform.position = focusedPosition;
        transform.localScale = focusedScale;

        foreach (var r in renderers) r.sortingOrder = focusedSortingOrder;
        
        ownCollider.enabled = false;
        SetInteractable(true);
    }
    
    public void Unfocus()
    {
        if (!isFocused) return;
        isFocused = false;

        transform.position = originalPosition;
        transform.localScale = originalScale;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sortingOrder = originalSortingOrders[i];

        ownCollider.enabled = true;
        SetInteractable(false);
    }
    
    private void SetInteractable(bool value)
    {
        foreach (var comp in componentsToEnableOnFocus)
        {
            comp.enabled = value;
        }
    }
}
