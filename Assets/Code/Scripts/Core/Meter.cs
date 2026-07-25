using UnityEngine;

/// <summary>
/// The ONLY money -> clock time converter in the game (DESIGN.md I3).
/// $1 = 1s via GameConfig.exchangeRate. Nothing else may add clock seconds
/// from money - route new systems through this instead of CountdownTimer directly.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Meter : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private MoneyService money;
    [SerializeField] private CountdownTimer clock;

    public bool Feed(int amount)
    {
        if (amount < config.meterMinFeed) return false;
        if (!money.TrySpend(amount, "meter feed")) return false;

        clock.AddSeconds(amount * config.exchangeRate, "meter");
        return true;
    }
}
