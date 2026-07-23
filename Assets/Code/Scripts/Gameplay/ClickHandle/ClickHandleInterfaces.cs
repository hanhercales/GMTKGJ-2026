using UnityEngine;

public interface IClickInputHandler
{
    void OnClickDown(Vector2 worldPos);
}

public interface IDragInputHandler
{
    void OnDragStart(Vector2 worldPos);
    void OnDragUpdate(Vector2 worldPos);
    void OnDragEnd(Vector2 worldPos);
}
