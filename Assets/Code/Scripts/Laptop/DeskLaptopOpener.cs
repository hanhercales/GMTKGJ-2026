using UnityEngine;

/// <summary>
/// Put this on the physical desk Laptop sprite. Clicking it opens the Laptop
/// UI panel (the home screen with the app icons). Closing happens via the
/// panel's own CloseButton, not by clicking the desk object again.
/// </summary>
public class DeskLaptopOpener : InteractableHandler
{
    [SerializeField] private GameObject laptopPanel;

    protected override void OnClicked(Vector2 worldPos)
    {
        laptopPanel.SetActive(true);
    }
}
