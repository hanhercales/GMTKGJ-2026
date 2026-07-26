using UnityEngine;

public class Sharpedo : MonoBehaviour, ICardEffect
{
    [SerializeField] private float duration = 30f;

    public void Apply() => Phone.Instance?.Silence(duration);
}
