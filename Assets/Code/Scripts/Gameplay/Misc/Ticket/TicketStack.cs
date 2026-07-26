using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PointerReceiver))]
[RequireComponent(typeof(Collider2D))]
public class TicketStack : MonoBehaviour, IPawnable
{
    [SerializeField] private ScratchTicket ticketPrefab;
    [SerializeField] private Vector2 spawnRangeMin = new Vector2(-4f, -3f);
    [SerializeField] private Vector2 spawnRangeMax = new Vector2(4f, 3f);
    [SerializeField] private TextMeshProUGUI payoutText;
    [SerializeField] private Button cashOutButton;
    [SerializeField] private GameObject[] GameObjectsToEnableOnFocus;
    
    [Header("Pawn")]
    [SerializeField] private int pawnValue = 20;
    
    public int PawnValue => pawnValue;
    
    private PointerReceiver receiver;
    
    private void Awake()
    {
        receiver = GetComponent<PointerReceiver>();
    }

    private void OnEnable() => receiver.ClickDown += HandleClick;
    private void OnDisable() => receiver.ClickDown -= HandleClick;

    private void HandleClick(Vector2 worldPos)
    {
        if (ticketPrefab == null) return;
        
        Vector2 spawnPos = GetRandomSpawnPosition(); 
        
        ScratchTicket prefab = Instantiate(ticketPrefab, spawnPos, Quaternion.identity);
        
        prefab.SetUIReferences(payoutText, cashOutButton);
        
        if (prefab.TryGetComponent(out DeskObjectFocus focus))
            focus.SetExtraObjects(GameObjectsToEnableOnFocus);
        
        if (!prefab.TryPurchase())
        {
            Destroy(prefab.gameObject);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float x = (float)(spawnRangeMin.x + RngService.Instance.Random.NextDouble() * (spawnRangeMax.x - spawnRangeMin.x));
        float y = (float)(spawnRangeMin.y + RngService.Instance.Random.NextDouble() * (spawnRangeMax.y - spawnRangeMin.y));
        return new Vector2(x, y);
    }
}
