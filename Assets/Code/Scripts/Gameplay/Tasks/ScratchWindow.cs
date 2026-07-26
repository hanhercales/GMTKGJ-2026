using System;
using UnityEngine;

public class ScratchWindow : HoldTask
{
    public event Action<ScratchWindow> Revealed;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Sprite assignedSymbol;

    protected override bool ResetAfterComplete => false;
    protected override bool ResetProgressOnRelease => false;
    
    public void SetSymbol(Sprite symbol) => assignedSymbol = symbol;
    
    protected override void ApplyReward() { }
    
    protected override void OnTaskCompleted()
    {
        if (spriteRenderer != null && assignedSymbol != null)
            spriteRenderer.sprite = assignedSymbol;
        
        Revealed?.Invoke(this);
    }

    public void ForceReveal()
    {
        if(IsCompleted) return;
        CompleteTask();
    }
}
