using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drag the pickaxe icon onto the ore block to hit it. Releasing over the
/// block fires Hit; releasing anywhere else is just a missed swing - the
/// pickaxe always snaps back to its resting spot, no penalty either way.
/// </summary>
public class PickaxeView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform oreTarget;

    private RectTransform _rect;
    private Vector2 _homePosition;

    public event Action Hit;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _homePosition = _rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_rect.parent, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            _rect.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(oreTarget, eventData.position, eventData.pressEventCamera))
            Hit?.Invoke();

        _rect.anchoredPosition = _homePosition;
    }
}
