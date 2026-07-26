using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overtime.Blackjack;

/// <summary>
/// One card slot: a face showing rank + colored suit symbol, a solid-color
/// back for face-down cards, or hidden entirely when the hand doesn't reach
/// this slot yet. No card sprite art exists in the project - this reads
/// purely off BlackjackGame's Card struct.
/// </summary>
public class CardSlotView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color faceColor = Color.white;
    [SerializeField] private Color backColor = new Color(0f, 0.1f, 0.5f);
    [SerializeField] private Color redSuitColor = new Color(0.75f, 0.1f, 0.1f);
    [SerializeField] private Color blackSuitColor = Color.black;

    public void ShowCard(Card card)
    {
        gameObject.SetActive(true);
        background.color = faceColor;
        label.text = $"{RankText(card.Rank)}{SuitSymbol(card.Suit)}";
        label.color = IsRed(card.Suit) ? redSuitColor : blackSuitColor;
    }

    public void ShowFaceDown()
    {
        gameObject.SetActive(true);
        background.color = backColor;
        label.text = "";
    }

    public void Hide() => gameObject.SetActive(false);

    private static bool IsRed(Suit suit) => suit == Suit.Hearts || suit == Suit.Diamonds;

    private static string SuitSymbol(Suit suit) => suit switch
    {
        Suit.Clubs => "♣",
        Suit.Diamonds => "♦",
        Suit.Hearts => "♥",
        Suit.Spades => "♠",
        _ => "?"
    };

    private static string RankText(int rank) => rank switch
    {
        1 => "A", 11 => "J", 12 => "Q", 13 => "K",
        _ => rank.ToString()
    };
}
