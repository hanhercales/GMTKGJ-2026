using UnityEngine;

public class CardDrop : InteractableHandler
{
    [Header("Click")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite clickedSprite;
    
    [Header("Drag")]
    [SerializeField] private float rotateSpeedDegPerSec = 180f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnClicked(Vector2 worldPos)
    {
        if (spriteRenderer.sprite != clickedSprite)
            spriteRenderer.sprite = clickedSprite;
    }

    protected override void OnDragMoved(Vector2 worldPos)
    {
        transform.position = worldPos;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.identity,
            rotateSpeedDegPerSec * Time.deltaTime);
    }
}
