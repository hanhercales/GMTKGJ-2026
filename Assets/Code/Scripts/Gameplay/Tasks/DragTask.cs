using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragTask : TaskBase, IDragInputHandler
{
    [Header("Settings")] 
    [SerializeField] private Transform handle;
    [SerializeField] private Vector2 dragAxis = Vector2.down;
    [SerializeField] private float requiredDistance = 2f;
    [SerializeField] private bool reset = true;
    
    private Vector3 startPos;
    private Vector2 pointerStartWPos;
    private float draggedDist;

    private void Awake()
    {
        if(handle == null) handle = transform;
        startPos = handle.position;
    }

    public void OnDragStart(Vector2 worldPos)
    {
        if (IsCompleted) return;
        pointerStartWPos = worldPos;
    }

    public void OnDragUpdate(Vector2 worldPos)
    {
        if(IsCompleted) return;

        Vector2 delta = worldPos - pointerStartWPos;
        float projected = Vector2.Dot(delta, dragAxis.normalized);
        draggedDist = Mathf.Clamp(projected, 0f, requiredDistance);
        
        handle.position = startPos + (Vector3)(dragAxis.normalized * draggedDist);

        if (draggedDist >= requiredDistance)
        {
            CompleteTask();
        }
    }

    public void OnDragEnd(Vector2 worldPos)
    {
        if (IsCompleted) return;

        if (reset)
        {
            draggedDist = 0f;
            handle.position = startPos;
        }
    }

    private Vector2 GetMouseWPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    protected override void OnTaskCompleted()
    {
        
    }
}
