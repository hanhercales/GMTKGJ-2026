using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PointerReceiver))]
public class Pawn : MonoBehaviour
{
    [SerializeField] private LayerMask pawnableLayers = ~0;
    
    private PointerReceiver receiver;
    
    private void Awake()
    {
        receiver = GetComponent<PointerReceiver>();
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }
    
    private void OnEnable() => receiver.DragUpdate += HandleDragUpdate;
    private void OnDisable() => receiver.DragUpdate -= HandleDragUpdate;
    
    private void HandleDragUpdate(Vector2 worldPos)
    {
        transform.position = worldPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & pawnableLayers) == 0) return;

        if (other.TryGetComponent(out IPawnable pawnable))
        {
            MoneyService.Instance.Add(pawnable.PawnValue, "pawn");
            Destroy(other.gameObject);
        }
    }
}
