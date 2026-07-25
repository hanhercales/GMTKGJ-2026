using UnityEngine;

/// <summary>
/// Single source of truth for every tuning value in the game (DESIGN.md I10, §5).
/// Only Ari edits this asset - Han requests value changes in chat (DESIGN.md §7.3).
/// No economy literals belong in any other script.
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Overtime/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Global (§5.1)")]
    public float startClock = 120f;
    public float winClock = 600f;
    public float loseClock = 0f;
    public int startMoney = 40;
    public float exchangeRate = 1f; // $1 = 1s
    public int meterMinFeed = 10;
    public float targetRunLengthMin = 210f;
    public float targetRunLengthMax = 300f;

    [Header("Mining (§5.2)")]
    public float blockDuration = 1f;
    public int[] blockPayout = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public int veinLength = 10;
    public float veinRespawn = 1.5f;

    [Header("Ticket - Triple Match (§5.3)")]
    public int tripleMatchCost = 25;
    public float tripleMatchScratchTime = 3f;
    [Range(0f, 1f)] public float tripleMatchZeroChance = 0.55f;
    [Range(0f, 1f)] public float tripleMatchMidChance = 0.35f;
    [Range(0f, 1f)] public float tripleMatchJackpotChance = 0.10f;
    public int tripleMatchMidPayout = 60;
    public int tripleMatchJackpotPayout = 200;

    [Header("Ticket - Lucky Nine (§5.3)")]
    public int luckyNineCost = 80;
    public float luckyNineScratchTime = 8f;
    [Range(0f, 1f)] public float luckyNineZeroChance = 0.70f;
    [Range(0f, 1f)] public float luckyNineMidChance = 0.20f;
    [Range(0f, 1f)] public float luckyNineJackpotChance = 0.10f;
    public int luckyNineMidPayout = 150;
    public int luckyNineJackpotPayout = 450;
    [Range(0f, 1f)] public float ticketAutoRevealThreshold = 0.6f;
    [Range(0f, 1f)] public float ticketAutoResolveThreshold = 0.8f;

    [Header("Card Box (§5.4)")]
    public int boxCost = 15;
    [Range(0f, 1f)] public float hitDropChance = 0.15f;
    public int handCap = 3;
    [Range(0f, 1f)] public float buffRatio = 0.55f;

    [Header("Card Durations (§5.4)")]
    public float doNotDisturbDuration = 30f;
    public float slowHandDuration = 15f;
    public float slowHandMultiplier = 0.75f;
    public float driverCrashDuration = 15f;
    public float connectionLostDuration = 20f;
    public float interestDuration = 20f;
    public float interestMultiplier = 1.5f;
    public float robocallsDuration = 30f;
    public float rushDuration = 15f;
    public float rushMultiplier = 1.25f;

    [Header("Blackjack (§5.5 / BLACKJACK.md §4)")]
    public float blackjackAnte = 15f;
    public float blackjackClockMargin = 10f;
    public int blackjackWinPayout = 120;
    public int blackjackPushPayout = 60;
    public int blackjackNaturalPayout = 200;
    public float blackjackRevealDelay = 0.6f;
    public float blackjackResultDelay = 1.2f;
    public float laptopSwitchTime = 1.0f;

    [Header("ITRS (§5.6)")]
    public float billInterval = 45f;
    [Range(0f, 1f)] public float billRate = 0.20f;
    public int billMinimum = 20;
    public float shortfallPenalty = 2f;

    [Header("Phone (§5.7)")]
    public float ringIntervalMin = 30f;
    public float ringIntervalMax = 60f;
    public float ringEffectMultiplier = 1.5f;
    public float answerTime = 1.5f;
    public float maxRingDuration = 10f;

    [Header("Annoyance Manager (§5.7)")]
    public float annoyanceMinGap = 12f;

    [Header("Egg Timer (§5.8)")]
    public float crankTime = 2f;
    public float eggTimerPayout = 3f;

    [Header("Dish Rack (§5.9)")]
    public float dishBasePayout = 2f;
    public float dishPlateDuration = 1.5f;
    [Range(0f, 1f)] public float dishFatigueDecay = 0.20f;
    public float dishPayoutFloor = 0.5f;

    [Header("Pawn Card (§5.10)")]
    public int pawnDishRack = 120;
    public int pawnEggTimer = 150;
    public int pawnCardBox = 180;
    public int pawnCoffeeMug = 50;

    [Header("Cat (§5.11)")]
    public float catIntervalMin = 40f;
    public float catIntervalMax = 70f;
    public float catSitDuration = 8f;
    public int catShooCost = 2;
}
