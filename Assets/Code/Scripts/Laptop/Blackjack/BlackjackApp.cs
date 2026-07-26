using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overtime.Blackjack;

/// <summary>
/// Blackjack as a laptop app. Ante is paid in CLOCK SECONDS, payout is MONEY.
/// This is the only time -> money converter in the game (DESIGN.md I3, 5.5).
///
/// This class owns: ante charging, payout, safety rails, UI.
/// BlackjackGame owns: cards and rules. Keep that split.
/// </summary>
public class BlackjackApp : LaptopApp
{
    [Header("Services")]
    [SerializeField] private GameConfig config;
    [SerializeField] private CountdownTimer clock;
    [SerializeField] private MoneyService money;
    [SerializeField] private RngService rng;

    [Header("UI")]
    [SerializeField] private Button dealButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private TMP_Text doubleButtonLabel;
    [SerializeField] private TMP_Text playerValueLabel;
    [SerializeField] private TMP_Text dealerValueLabel;
    [SerializeField] private TMP_Text resultLabel;
    [SerializeField] private TMP_Text anteLabel;
    [SerializeField] private TMP_Text payoutLabel;
    [SerializeField] private TMP_Text spentLabel;
    [SerializeField] private TMP_Text receivedLabel;
    [SerializeField] private CardRowView playerRow;
    [SerializeField] private CardRowView dealerRow;

    private BlackjackGame _game;
    private bool _resolving;
    private float _totalSpent;
    private int _totalReceived;

    // Disabled by the "Connection Lost" debuff. Does NOT disable mining.
    public bool Banned { get; private set; }

    private void Start()
    {
        // Built in Start, not Awake: rng.Random is set in RngService.Awake(),
        // and Unity doesn't guarantee Awake() order across components.
        _game = new BlackjackGame(rng.Random);

        dealButton.onClick.AddListener(OnDeal);
        hitButton.onClick.AddListener(OnHit);
        standButton.onClick.AddListener(OnStand);
        doubleButton.onClick.AddListener(OnDouble);

        // Static for the session - ante/payouts don't change mid-run.
        payoutLabel.text = $"win ${config.blackjackWinPayout} · push ${config.blackjackPushPayout} · bj ${config.blackjackNaturalPayout}";
        doubleButtonLabel.text = $"DOUBLE -{config.blackjackAnte:0}s";

        Refresh();
    }

    // ---------- Safety rails ----------

    /// <summary>
    /// The ante must never be able to kill the player outright.
    /// Requires a margin above the ante so a loss is survivable.
    /// </summary>
    private bool CanAffordAnte =>
        clock.CurrentSeconds >= config.blackjackAnte + config.blackjackClockMargin;

    private bool Available => !Banned && !_resolving;

    // ---------- Actions ----------

    private void OnDeal()
    {
        if (!Available || _game.State != HandState.Idle) return;
        if (!CanAffordAnte)
        {
            Flash("NOT ENOUGH TIME");
            return;
        }

        clock.Spend(config.blackjackAnte, "blackjack ante");
        _totalSpent += config.blackjackAnte;
        _game.Deal();

        if (_game.State == HandState.Resolved)
            StartCoroutine(Resolve());   // natural on the deal

        Refresh();
    }

    private void OnHit()
    {
        if (!Available) return;

        _game.Hit();
        if (_game.State == HandState.Resolved) StartCoroutine(Resolve());
        Refresh();
    }

    private void OnStand()
    {
        if (!Available) return;

        _game.Stand();
        StartCoroutine(Resolve());
        Refresh();
    }

    private void OnDouble()
    {
        if (!Available || !_game.CanDouble) return;

        // Doubling costs a SECOND ante in clock seconds. This is the most
        // expensive decision in the game and it should feel like it.
        if (!CanAffordAnte)
        {
            Flash("NOT ENOUGH TIME");
            return;
        }

        clock.Spend(config.blackjackAnte, "blackjack double");
        _totalSpent += config.blackjackAnte;
        _game.DoubleDown();
        StartCoroutine(Resolve());
        Refresh();
    }

    // ---------- Resolution ----------

    private IEnumerator Resolve()
    {
        _resolving = true;
        Refresh();

        // Reveal beat. Keep this SHORT - the clock is running and dead
        // time on a resolved hand feels like theft.
        yield return new WaitForSeconds(config.blackjackRevealDelay);

        int payout = _game.Payout(
            config.blackjackWinPayout,
            config.blackjackPushPayout,
            config.blackjackNaturalPayout);

        if (payout > 0)
        {
            money.Add(payout, "blackjack");
            _totalReceived += payout;
        }

        resultLabel.text = payout > 0
            ? $"{_game.ResultText}  +${payout}"
            : _game.ResultText;

        yield return new WaitForSeconds(config.blackjackResultDelay);

        _game.Reset();
        _resolving = false;
        Refresh();
    }

    // ---------- Debuff hook ----------

    /// <summary>Called by the card system. Blocks blackjack ONLY, never mining.</summary>
    public void ApplyBan(float seconds) => StartCoroutine(BanRoutine(seconds));

    private IEnumerator BanRoutine(float seconds)
    {
        Banned = true;
        Refresh();
        yield return new WaitForSeconds(seconds);
        Banned = false;
        Refresh();
    }

    // ---------- App lifecycle ----------

    public override void OnAppOpened() => Refresh();

    public override void OnAppClosed()
    {
        // Closing the app mid-hand FORFEITS the ante and the hand.
        // Intentional: the player already paid, and letting them park a
        // live hand while they mine would break the lockout.
        if (_game.State == HandState.PlayerTurn)
        {
            _game.Reset();
            _resolving = false;
            StopAllCoroutines();
        }
    }

    // ---------- UI ----------

    private void Refresh()
    {
        bool idle = _game.State == HandState.Idle && !_resolving;
        bool playing = _game.State == HandState.PlayerTurn && !_resolving;

        dealButton.gameObject.SetActive(idle);
        hitButton.gameObject.SetActive(playing);
        standButton.gameObject.SetActive(playing);
        doubleButton.gameObject.SetActive(playing);

        dealButton.interactable   = idle && Available && CanAffordAnte;
        hitButton.interactable    = playing && _game.CanHit;
        standButton.interactable  = playing;
        doubleButton.interactable = playing && _game.CanDouble && CanAffordAnte;

        anteLabel.text = Banned
            ? "CONNECTION LOST"
            : $"ANTE {config.blackjackAnte:0}s";

        spentLabel.text = $"Spent {_totalSpent:0}s";
        receivedLabel.text = $"Received ${_totalReceived}";

        playerRow.Render(_game.PlayerHand, hideIndex: -1);
        dealerRow.Render(_game.DealerHand, hideIndex: _game.DealerHoleHidden ? 1 : -1);

        playerValueLabel.text = _game.PlayerHand.Count == 0
            ? ""
            : "YOU " + (BlackjackGame.IsSoft(_game.PlayerHand) ? "soft " : "") + _game.PlayerValue;

        dealerValueLabel.text = _game.DealerHand.Count == 0
            ? ""
            : "DEALER " + (_game.DealerHoleHidden ? _game.DealerUpValue + " + ?" : _game.DealerValue.ToString());

        if (idle) resultLabel.text = "";
    }

    private void Flash(string message)
    {
        resultLabel.text = message;
    }
}
