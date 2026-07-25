using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PointerReceiver))]
public class ClickTask : TaskBase
{
    [Header("Settings")] [SerializeField] private int requiredClicks = 1;
    
    private int currentClicks = 0;
    private PointerReceiver receiver;
    
    private void Awake() => receiver = GetComponent<PointerReceiver>();

    private void OnEnable() => receiver.ClickDown += HandleClick;

    private void OnDisable() => receiver.ClickDown -= HandleClick;

    private void HandleClick(Vector2 worldPos)
    {
        if(IsCompleted) return;
        currentClicks++;
        
        if(currentClicks >= requiredClicks) CompleteTask();
    }
}
