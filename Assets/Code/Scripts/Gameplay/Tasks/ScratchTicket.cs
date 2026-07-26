using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScratchTicket : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cost = 25;
    [SerializeField] private float scratchTime = 3f;
    [SerializeField] private ScratchWindow[] windows;

    [Header("Reward")]
    [SerializeField] private int baseReward = 40;

    [Header("Symbols")]
    [SerializeField] private Sprite[] symbolPool;

    [Header("Auto Resolve")]
    [SerializeField, Range(0f, 1f)] private float autoResolveThreshold = 0.8f;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI payoutText;
    [SerializeField] private Button cashOutButton;

    private int revealedCount;
    private int rolledPayout;
    private bool hasAutoRevealed;
    private bool isResolved;
    private bool isPurchased;

    private void Awake()
    {
        foreach (var window in windows)
        {
            window.SetHoldDuration(scratchTime / windows.Length);
            window.Revealed += HandleWindowReveal;
        }
        
        cashOutButton.onClick.AddListener(HandleCashOutClicked);
        
        RollAndAssignSymbols();
        UpdatePayoutDisplay();
    }
    
    private void OnDestroy()
    {
        if (cashOutButton != null) cashOutButton.onClick.RemoveListener(HandleCashOutClicked);
    }

    public bool TryPurchase()
    {
        if(isPurchased) return false;
        if (!MoneyService.Instance.TrySpend(cost, "scratch ticket")) return false;
        
        isPurchased = true;
        return true;
    }

    private void RollAndAssignSymbols()
    {
        if (symbolPool.Length == 0) return;

        Sprite[] rolledSymbols = new Sprite[windows.Length];
        Dictionary<Sprite, int> counts = new Dictionary<Sprite, int>();

        for (int i = 0; i < windows.Length; i++)
        {
            Sprite symbol = symbolPool[RngService.Instance.Random.Next(symbolPool.Length)];
            rolledSymbols[i] = symbol;
            counts[symbol] = counts.TryGetValue(symbol, out int c) ? c + 1 : 1;
        }

        int maxCount = 0;
        foreach (var kvp in counts)
        {
            if (kvp.Value > maxCount) maxCount = kvp.Value;
        }

        rolledPayout = baseReward * Mathf.Max(0, maxCount - 1);

        for (int i = 0; i < windows.Length; i++)
            windows[i].SetSymbol(rolledSymbols[i]);
    }

    private void HandleWindowReveal(ScratchWindow window)
    {
        if (isResolved) return;
        
        revealedCount++;
        UpdatePayoutDisplay();
        
        float clearedRatio = (float)revealedCount / windows.Length;

        if (clearedRatio >= autoResolveThreshold)
            ResolveOutcome();
    }

    private void HandleCashOutClicked()
    {
        if (isResolved) return;

        int currentAmount = Mathf.RoundToInt((float)rolledPayout * revealedCount / windows.Length);
        LockTicket(currentAmount, "scratch ticket cash out");
        
        Destroy(gameObject);
    }

    private void UpdatePayoutDisplay()
    {
        if(payoutText == null) return;
        
        int displayedAmount = Mathf.RoundToInt((float)rolledPayout * revealedCount / windows.Length);
        payoutText.text = $"${displayedAmount}";
    }

    private void ResolveOutcome()
    {
        if (isResolved) return;
        isResolved = true;
        
        foreach (var window in windows)
        {
            if (!window.IsCompleted) window.ForceReveal();
        }

        if (rolledPayout > 0) MoneyService.Instance.Add(rolledPayout, "scratch ticket payout");
        if (payoutText != null) payoutText.text = $"${rolledPayout}";
    }

    private void LockTicket(int amount, string reason)
    {
        isResolved = true;

        if (amount > 0) MoneyService.Instance.Add(amount, reason);

        if (payoutText != null) payoutText.text = $"${amount}";
        if (cashOutButton != null) cashOutButton.interactable = false;
    }
}
