using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PointerReceiver))]
public class ThrowTask : TaskBase
{
    [Header("Settings")]
    [SerializeField] private Transform pullHandle;
    [SerializeField] private float maxPullDistance = 1f;
    [SerializeField] private float maxLaunchForce = 10f;
    [SerializeField] private string targetTag = "Target";
    [SerializeField] private float settleVelocityThreshold = 0.05f;
    [SerializeField] private float settleGraceTime = 0.15f;
    
    private PointerReceiver receiver;
    private Rigidbody2D rb;
    private Vector3 orgPos;
    private Vector3 handleOrgLocalPos;
    private bool isLaunched;
    private float settleTimer;

    private void Awake()
    {
        receiver = GetComponent<PointerReceiver>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        orgPos = transform.position;
        handleOrgLocalPos = pullHandle.localPosition;
    }
    
    private void OnEnable()
    {
        receiver.DragStart += HandleDragStart;
        receiver.DragUpdate += HandleDragUpdate;
        receiver.DragEnd += HandleDragEnd;
    }

    private void OnDisable()
    {
        receiver.DragStart -= HandleDragStart;
        receiver.DragUpdate -= HandleDragUpdate;
        receiver.DragEnd -= HandleDragEnd;
    }

    private void Update()
    {
        if (isLaunched && !IsCompleted)
            CheckSettle();
    }

    private void CheckSettle()
    {
        if (rb.linearVelocity.magnitude > settleVelocityThreshold)
        {
            settleTimer = 0f;
            return;
        }
        
        settleTimer += Time.deltaTime;
        if (settleTimer < settleGraceTime) return;
        
        isLaunched = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        orgPos = transform.position;
    }

    public void HandleDragStart(Vector2 worldPos)
    {
        if (IsCompleted || isLaunched) return;

        pullHandle.gameObject.SetActive(true);
        CountdownTimer.Instance.PauseCountdown(); 
    }

    public void HandleDragUpdate(Vector2 worldPos)
    {
        if (IsCompleted || isLaunched) return;
        
        Vector2 pullVector = worldPos - (Vector2)orgPos;
        pullVector = Vector2.ClampMagnitude(pullVector, maxPullDistance);
        
        pullHandle.position = transform.position + (Vector3)pullVector;
    }

    public void HandleDragEnd(Vector2 worldPos)
    {
        CountdownTimer.Instance.ResumeCountdown();
        
        if (IsCompleted || isLaunched) return;
        
        Vector2 pullVector = (Vector2)pullHandle.position - (Vector2)transform.position;
        float pullPercent = pullVector.magnitude / maxPullDistance;
        Vector2 launchVelocity = -pullVector.normalized * (pullPercent * maxLaunchForce);
        
        isLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = launchVelocity;
        rb.angularVelocity = Random.Range(-360f, 360f) * pullPercent;
        
        pullHandle.localPosition = handleOrgLocalPos;
        pullHandle.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsCompleted) return;
        
        if (other.collider.CompareTag(targetTag))
            CompleteTask();
    }

    protected override void OnTaskCompleted()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
