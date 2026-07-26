using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PointerReceiver))]
public class HoldTask : TaskBase, IPawnable
{
    [Header("Settings")]
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float timePayout = 3f;
    
    [Header("Pawn")]
    [SerializeField] private int pawnValue = 20;
    
    public int PawnValue => pawnValue;

    private PointerReceiver receiver;
    private float holdTimer;
    private bool isHolding;

    protected override bool ResetAfterComplete => true;
    protected virtual bool ResetProgressOnRelease => true;
    public void SetHoldDuration(float duration) => holdDuration = duration;
    
    protected virtual void Awake()
    {
        receiver = GetComponent<PointerReceiver>();
    }

    protected virtual void OnEnable()
    {
        receiver.DragStart += HandleHoldStart;
        receiver.DragUpdate += HandleHoldUpdate;
        receiver.DragEnd += HandleHoldEnd;
    }

    protected virtual void OnDisable()
    {
        receiver.DragStart -= HandleHoldStart;
        receiver.DragUpdate -= HandleHoldUpdate;
        receiver.DragEnd -= HandleHoldEnd;
    }

    private void HandleHoldStart(Vector2 worldPos)
    {
        isHolding = true;
    }

    private void HandleHoldUpdate(Vector2 worldPos)
    {
        if(!isHolding) return;
        
        holdTimer += Time.deltaTime;

        if (holdTimer >= holdDuration)
        {
            isHolding = false;
            CompleteTask();
        }
    }

    private void HandleHoldEnd(Vector2 worldPos)
    {
        isHolding = false;
        if(ResetProgressOnRelease)
            holdTimer = 0f;
    }

    protected override void ApplyReward()
    {
        CountdownTimer.Instance.AddTime(timePayout);
    }
}
