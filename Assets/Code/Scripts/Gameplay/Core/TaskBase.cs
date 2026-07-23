using UnityEngine;

public abstract class TaskBase : MonoBehaviour
{
    [SerializeField] private float timeReward = 5f;
    
    public bool IsCompleted { get;  private set; }
    
    protected void CompleteTask()
    {
        if(IsCompleted) return;
        IsCompleted = true;
        
        CountdownTimer.Instance.AddTime(timeReward);
        OnTaskCompleted();
    }

    protected virtual void OnTaskCompleted() { }
}
