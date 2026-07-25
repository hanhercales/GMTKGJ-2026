using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The laptop hosts multiple apps (Mining, Blackjack) on one screen. Only ONE
/// app is active at a time - that mutual exclusion is physical rather than a
/// rule, which is the main win of putting blackjack on the laptop (BLACKJACK.md §1).
///
/// CRITICAL (DESIGN.md §5.5): the laptop screen must NOT go fullscreen. The
/// desk has to stay visible or the phone, cat and mail tray stop existing.
/// </summary>
public class LaptopController : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private List<LaptopApp> apps = new List<LaptopApp>();
    [SerializeField] private GameObject switchingOverlay;

    private LaptopApp _current;
    private bool _switching;

    // Set by a whole-laptop debuff, if one is ever added. Default design does
    // NOT use this - see BLACKJACK.md §2.1 (Dead Battery split into two apps).
    public bool Disabled { get; set; }

    private void Start()
    {
        foreach (var app in apps)
            app.gameObject.SetActive(false);

        if (apps.Count > 0) OpenImmediate(apps[0]);
    }

    public void RequestOpen(LaptopApp app)
    {
        if (Disabled || _switching || app == _current) return;
        StartCoroutine(SwitchRoutine(app));
    }

    public void RequestOpen(string appName)
    {
        var app = apps.Find(a => a.AppName == appName);
        if (app != null) RequestOpen(app);
    }

    /// <summary>
    /// App switching costs real time (config.laptopSwitchTime). Deliberate:
    /// hopping between mining and blackjack should have a price.
    /// </summary>
    private IEnumerator SwitchRoutine(LaptopApp next)
    {
        _switching = true;
        if (switchingOverlay != null) switchingOverlay.SetActive(true);

        if (_current != null)
        {
            _current.OnAppClosed();
            _current.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(config.laptopSwitchTime);

        OpenImmediate(next);

        if (switchingOverlay != null) switchingOverlay.SetActive(false);
        _switching = false;
    }

    private void OpenImmediate(LaptopApp app)
    {
        _current = app;
        app.gameObject.SetActive(true);
        app.OnAppOpened();
    }
}
