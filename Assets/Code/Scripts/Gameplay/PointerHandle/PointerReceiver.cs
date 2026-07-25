using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PointerReceiver : MonoBehaviour, IPointerInputHandler
{
    public event Action<Vector2> ClickDown;
    public event Action<Vector2> DragStart;
    public event Action<Vector2> DragUpdate;
    public event Action<Vector2> DragEnd;
    
    public void OnClickDown(Vector2 worldPos) => ClickDown?.Invoke(worldPos);
    public void OnDragStart(Vector2 worldPos) => DragStart?.Invoke(worldPos);
    public void OnDragUpdate(Vector2 worldPos) => DragUpdate?.Invoke(worldPos);
    public void OnDragEnd(Vector2 worldPos) => DragEnd?.Invoke(worldPos);
}
