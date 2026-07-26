using System;
using UnityEngine;

/// <summary>
/// Interruption desk object (DESIGN.md §5.7). Rings on a random real-time interval;
/// while ringing the clock ticks at ringEffectMultiplier via CountdownTimer.TickMultiplier
/// (the one field phone/cards are allowed to write). Clicking starts "answering" - the
/// penalty keeps running for answerTime, then the ring stops. Routes through AnnoyanceManager
/// so it never lands within annoyanceMinGap of another interruption (I8).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Phone : InteractableHandler
{
    public static Phone Instance { get; private set; }

    [SerializeField] private GameConfig config;
    [SerializeField] private CountdownTimer clock;
    [SerializeField] private AnnoyanceManager annoyance;
    [SerializeField] private GameObject ringingVisual;

    public bool IsRinging { get; private set; }
    public event Action OnRingStart;
    public event Action OnRingEnd;

    private float nextRingTimer;
    private float ringElapsed;
    private float answeringElapsed = -1f;

    private bool silenced;
    private float silenceTimer;

    private bool robocallsActive;
    private float robocallsTimer;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        ScheduleNextRing();
    }

    private void ScheduleNextRing()
    {
        float min = config.ringIntervalMin;
        float max = config.ringIntervalMax;
        if (robocallsActive) { min *= 0.5f; max *= 0.5f; }

        nextRingTimer = min + (float)RngService.Instance.Random.NextDouble() * (max - min);
    }

    private void Update()
    {
        if (!clock.IsRunning || clock.IsGameOver) return;

        if (silenced)
        {
            silenceTimer -= Time.deltaTime;
            if (silenceTimer <= 0f) silenced = false;
        }

        if (robocallsActive)
        {
            robocallsTimer -= Time.deltaTime;
            if (robocallsTimer <= 0f) robocallsActive = false;
        }

        if (!IsRinging)
        {
            if (silenced) return;

            nextRingTimer -= Time.deltaTime;
            if (nextRingTimer <= 0f) TryStartRing();
            return;
        }

        ringElapsed += Time.deltaTime;

        if (answeringElapsed >= 0f)
        {
            answeringElapsed += Time.deltaTime;
            if (answeringElapsed >= config.answerTime) StopRing();
        }
        else if (ringElapsed >= config.maxRingDuration)
        {
            StopRing();
        }
    }

    private void TryStartRing()
    {
        if (annoyance != null && !annoyance.TryBegin("phone"))
        {
            nextRingTimer = 1f; // retry shortly rather than skipping this ring
            return;
        }

        IsRinging = true;
        ringElapsed = 0f;
        answeringElapsed = -1f;
        clock.TickMultiplier = config.ringEffectMultiplier;

        if (ringingVisual) ringingVisual.SetActive(true);
        OnRingStart?.Invoke();
    }

    private void StopRing()
    {
        IsRinging = false;
        clock.TickMultiplier = 1f;

        if (ringingVisual) ringingVisual.SetActive(false);
        annoyance?.End("phone");

        ScheduleNextRing();
        OnRingEnd?.Invoke();
    }

    protected override void OnClicked(Vector2 worldPos)
    {
        if (IsRinging && answeringElapsed < 0f)
            answeringElapsed = 0f;
    }

    /// <summary>Do Not Disturb buff - silences the phone for duration, cutting off an active ring immediately.</summary>
    public void Silence(float duration)
    {
        silenced = true;
        silenceTimer = duration;
        if (IsRinging) StopRing();
    }

    /// <summary>Robocalls debuff - rings roughly twice as often for duration.</summary>
    public void StartRobocalls(float duration)
    {
        robocallsActive = true;
        robocallsTimer = duration;
    }
}
