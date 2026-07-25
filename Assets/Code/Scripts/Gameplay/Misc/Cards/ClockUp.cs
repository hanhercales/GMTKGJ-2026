using UnityEngine;

public class ClockUp : MonoBehaviour, ICardEffect
{
    [SerializeField] private int boostStep = 3;
    [SerializeField] private float duration = 5f;

    public void Apply() => CountdownTimer.Instance.OverrideSpeedMult(boostStep, duration);
}
