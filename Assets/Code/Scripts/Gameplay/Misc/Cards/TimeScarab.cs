using UnityEngine;

public class TimeScarab : MonoBehaviour, ICardEffect
{
    [SerializeField] private float duration = 3f;

    public void Apply() => CountdownTimer.Instance.OverrideSpeedMult(0, duration);
}
