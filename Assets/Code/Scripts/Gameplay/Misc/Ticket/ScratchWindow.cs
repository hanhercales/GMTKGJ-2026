using System;
using UnityEngine;

public class ScratchWindow : HoldTask
{
    public event Action<ScratchWindow> Revealed;
    
    private Sprite assignedSymbol;
    private Sprite originalSprite;
    private Color originalColor;

    public Sprite AssignedSymbol => assignedSymbol;
    protected override bool ResetAfterComplete => false;
    protected override bool ResetProgressOnRelease => false;

    protected override void Awake()
    {
        base.Awake();
        
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
            originalColor = spriteRenderer.color;
        }
    }

    public void SetSymbol(Sprite symbol) => assignedSymbol = symbol;

    public void ResetWindow()
    {
        ResetHoldState();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.color = originalColor;
        }
    }
    
    protected override void ApplyReward() { }
    
    protected override void OnTaskCompleted()
    {
        if (spriteRenderer != null && assignedSymbol != null)
        {
            spriteRenderer.sprite = assignedSymbol;
            spriteRenderer.color = Color.white;
        }
        
        Revealed?.Invoke(this);
    }

    public void ForceReveal()
    {
        if(IsCompleted) return;
        CompleteTask();
    }
}
