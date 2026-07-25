using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Overtime.Blackjack;

/// <summary>
/// Renders one hand as a row of fixed slots. No playing-card sprite deck exists
/// in the project yet, so this reads cards as text (e.g. "AS", "10H") - swap the
/// slot prefab for Image-based cards once art lands, the Render() contract won't change.
/// </summary>
public class CardRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text[] cardSlots;

    public void Render(IReadOnlyList<Card> hand, int hideIndex)
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i >= hand.Count)
            {
                cardSlots[i].gameObject.SetActive(false);
                continue;
            }

            cardSlots[i].gameObject.SetActive(true);
            cardSlots[i].text = i == hideIndex ? "??" : hand[i].ToString();
        }
    }
}
