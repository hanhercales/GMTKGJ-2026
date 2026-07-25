using UnityEngine;

public class Invisible : MonoBehaviour, ICardEffect
{
    [SerializeField] private float duration = 4f;

    public void Apply() => CountdownTimer.Instance.HideDisplayFor(duration);
}
