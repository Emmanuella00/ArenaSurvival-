using TMPro;
using UnityEngine;

// =====================================================================
// UIManager - the HUD (health, ammo, wave, score) and the end screen.
//
// OBSERVER PATTERN (subscriber): listens to Health.OnHealthChanged and
//   AmmoSystem.OnAmmoChanged. Health and AmmoSystem do not know the UI
//   exists - they just raise events. This keeps game logic and UI separate.
// =====================================================================
public class UIManager : MonoBehaviour
{
    [Header("What to listen to")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private ShootController shooter;

    [Header("HUD texts")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text scoreText;

    [Header("End screen")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;

    private void Start()
    {
        // OBSERVER: subscribe to the events
        playerHealth.OnHealthChanged += OnHealthChanged;
        shooter.Ammo.OnAmmoChanged += OnAmmoChanged;

        // Draw the starting values once (events only fire on changes)
        OnHealthChanged(playerHealth.CurrentHealth);
        OnAmmoChanged(shooter.Ammo.Magazine, shooter.Ammo.Reserve);
        UpdateScore(0);
        endPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // OBSERVER: unsubscribe so no event calls a destroyed UI
        if (playerHealth != null) playerHealth.OnHealthChanged -= OnHealthChanged;
        if (shooter != null && shooter.Ammo != null) shooter.Ammo.OnAmmoChanged -= OnAmmoChanged;
    }

    private void OnHealthChanged(float value)
    {
        healthText.text = "Health: " + Mathf.CeilToInt(value);
    }

    private void OnAmmoChanged(int magazine, int reserve)
    {
        ammoText.text = "Ammo: " + magazine + " / " + reserve;
    }

    public void UpdateWave(int wave, int totalWaves)
    {
        waveText.text = "Wave " + wave + " / " + totalWaves;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowEndScreen(bool won, int score)
    {
        endPanel.SetActive(true);
        endTitleText.text = (won ? "YOU SURVIVED!" : "GAME OVER") + "\nScore: " + score;
    }

    // Hooked to the Restart button's OnClick in the Inspector
    public void OnRestartButton()
    {
        GameManager.Instance.Restart();
    }
}
