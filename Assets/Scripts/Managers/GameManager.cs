using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// GameManager - score, game over, win and restart.
//
// SINGLETON PATTERN: exactly one GameManager exists, reachable from any
//   script through GameManager.Instance (e.g. Enemy calls
//   GameManager.Instance.AddScore() when it dies).
// OBSERVER PATTERN (subscriber): listens to the player's Health.OnDied.
// ENCAPSULATION: score and gameEnded are private; Instance has a
//   private setter so no other script can replace the manager.
// =====================================================================
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Health playerHealth;
    [SerializeField] private UIManager ui;

    private int score;
    private bool gameEnded;

    public int Score { get { return score; } }
    public bool GameEnded { get { return gameEnded; } }

    private void Awake()
    {
        // SINGLETON: if another GameManager already exists, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;   // make sure the game is unpaused after a restart
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnDied += GameOver;    // OBSERVER: subscribe
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnDied -= GameOver;    // always unsubscribe
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void AddScore(int points)
    {
        if (gameEnded) return;
        score += points;
        ui.UpdateScore(score);
    }

    public void GameOver()
    {
        EndGame(false);
    }

    public void Win()
    {
        EndGame(true);
    }

    private void EndGame(bool won)
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;   // freeze the world

        // Turn off movement, looking and shooting (every script on the player except Health)
        foreach (MonoBehaviour script in playerHealth.GetComponents<MonoBehaviour>())
        {
            if (script != playerHealth) script.enabled = false;
        }

        // Free the mouse so the Restart button can be clicked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ui.ShowEndScreen(won, score);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
