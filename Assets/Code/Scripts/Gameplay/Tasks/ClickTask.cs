using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class ClickTask : TaskBase, IClickInputHandler
{
    [Header("Settings")] [SerializeField] private int requiredClicks = 1;
    
    private Collider2D col;
    
    public int CurrentClicks { get;  private set; }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void OnClickDown(Vector2 worldPos)
    {
        if (IsCompleted) return;

        CurrentClicks++;

        if (CurrentClicks >= requiredClicks)
        {
            CompleteTask();
        }
    }

    protected override void OnTaskCompleted()
    {
        
    }
}
