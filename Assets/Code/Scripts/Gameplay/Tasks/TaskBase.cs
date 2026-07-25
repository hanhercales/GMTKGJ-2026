using UnityEngine;
using UnityEngine.Serialization;

public abstract class TaskBase : MonoBehaviour
{
    // Renamed from timeReward: tasks pay MONEY now. Only the Meter converts
    // money -> clock time (DESIGN.md I3) - see MoneyService.
    [FormerlySerializedAs("timeReward")]
    [SerializeField] private int moneyReward = 5;

    public bool IsCompleted { get;  private set; }

    protected void CompleteTask()
    {
        if(IsCompleted) return;
        IsCompleted = true;

        MoneyService.Instance.Add(moneyReward, "task");
        OnTaskCompleted();
    }

    protected virtual void OnTaskCompleted() { }
}
