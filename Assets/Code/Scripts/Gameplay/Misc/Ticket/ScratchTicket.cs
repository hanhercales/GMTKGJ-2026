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
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI payoutText;
    [SerializeField] private Button cashOutButton;
    
    private DeskObjectFocus deskObjectFocus;
    private Dictionary<Sprite, int> revealedCounts = new Dictionary<Sprite, int>();
    private int revealedCount;
    private bool isResolved;
    private bool isPurchased;

    private void Awake()
    {
        deskObjectFocus = GetComponent<DeskObjectFocus>();
        
        foreach (var window in windows)
            window.SetHoldDuration(scratchTime / windows.Length);
    }

    private void OnEnable()
    {
        revealedCount = 0;
        isResolved = false;
        isPurchased = false;
        revealedCounts.Clear();

        RollAndAssignSymbols();

        foreach (var window in windows)
        {
            window.ResetWindow();
            window.Revealed += HandleWindowReveal;
        }

        if (cashOutButton != null)
        {
            cashOutButton.onClick.RemoveAllListeners();
            cashOutButton.onClick.AddListener(HandleCashOutClicked);
        }
        
        UpdatePayoutDisplay();
    }

    private void OnDisable()
    {
        foreach (var window in windows)
            window.Revealed -= HandleWindowReveal;

        if (cashOutButton != null)
            cashOutButton.onClick.RemoveListener(HandleCashOutClicked);
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

        for (int i = 0; i < windows.Length; i++)
        {
            Sprite symbol = symbolPool[RngService.Instance.Random.Next(symbolPool.Length)];
            windows[i].SetSymbol(symbol);
        }
    }

    private void HandleWindowReveal(ScratchWindow window)
    {
        if (isResolved) return;
        
        revealedCount++;
        
        Sprite symbol = window.AssignedSymbol;
        revealedCounts[symbol] = revealedCounts.TryGetValue(symbol, out int c) ? c + 1 : 1;
        
        UpdatePayoutDisplay();
        
        if (revealedCount >= windows.Length) 
            ResolveOutcome();
    }

    private void HandleCashOutClicked()
    {
        if (!isResolved)
        {
            int currentAmount = GetCurrentReward();
            LockTicket(currentAmount, "scratch ticket cash out");
        }
        
        if (deskObjectFocus != null) deskObjectFocus.Unfocus();
        gameObject.SetActive(false);
    }

    private int GetCurrentReward()
    {
        int maxCount = 0;
        foreach (var kvp in revealedCounts)
            if (kvp.Value > maxCount) maxCount = kvp.Value;

        return baseReward * Mathf.Max(0, maxCount - 1);
    }

    private void UpdatePayoutDisplay()
    {
        if(payoutText == null) return;
        
        payoutText.text = $"${GetCurrentReward()}";
    }

    private void ResolveOutcome()
    {
        if (isResolved) return;

        LockTicket(GetCurrentReward(), "scratch ticket payout");
    }

    private void LockTicket(int amount, string reason)
    {
        isResolved = true;

        if (amount > 0) MoneyService.Instance.Add(amount, reason);

        if (payoutText != null) payoutText.text = $"${amount}";
    }
}
