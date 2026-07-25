using UnityEngine;

public class CardItem : InteractableHandler
{
    [Header("Click")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite clickedSprite;
    
    [Header("Drag")]
    [SerializeField] private float rotateSpeedDegPerSec = 180f;
    
    [Header("Rewards/Penalties")]
    [SerializeField] private float amount = 5f;

    protected override void OnClicked(Vector2 worldPos)
    {
        if (spriteRenderer.sprite != clickedSprite)
        {
            spriteRenderer.sprite = clickedSprite;
            if (amount >= 0f) CountdownTimer.Instance.AddTime(amount);
            else CountdownTimer.Instance.SubstractTime(-amount);
            
            foreach (var effect in GetComponents<ICardEffect>())
                effect.Apply();
        }
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
