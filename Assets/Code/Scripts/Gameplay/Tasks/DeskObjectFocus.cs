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
    
    private GameObject[] gameObjectsToEnableOnFocus;
    private PointerReceiver receiver;
    private SpriteRenderer[] renderers;
    private Collider2D ownCollider;
    
    
    public void SetExtraObjects(GameObject[] objects) => gameObjectsToEnableOnFocus = objects; 
    
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
        CountdownTimer.Instance.PauseCountdown();
        
        if(isFocused)  return;
        isFocused = true;
        
        transform.position = focusedPosition;
        transform.localScale = focusedScale;
        
        int baseOffset = focusedSortingOrder - originalSortingOrders[0];

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sortingOrder = originalSortingOrders[i] + baseOffset;
        
        ownCollider.enabled = false;
        SetInteractable(true);

        foreach (var go in gameObjectsToEnableOnFocus)
            if (go != null) go.SetActive(true);
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
        
        foreach (var go in gameObjectsToEnableOnFocus)
            if (go != null) go.SetActive(false);
        
        CountdownTimer.Instance.ResumeCountdown();
    }
    
    private void SetInteractable(bool value)
    {
        foreach (var comp in componentsToEnableOnFocus)
        {
            comp.enabled = value;
        }
    }
}
