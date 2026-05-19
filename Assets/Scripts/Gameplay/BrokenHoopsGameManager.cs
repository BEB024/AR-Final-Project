using System.Collections;
using UnityEngine;
using TMPro;

public class BrokenHoopsGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallSpawnManager ballSpawnManager;
    [SerializeField] private GameplayUIManager uiManager;
    [SerializeField] private TrickShotChallengeManager trickShotChallengeManager;

    [Header("Direct UI Fallback")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text instructionText;

    [Header("Game State")]
    [SerializeField] private int score;
    [SerializeField] private float remainingTime;
    [SerializeField] private bool gameRunning;
    [SerializeField] private bool gamePaused;

    public int Score => score;
    public float RemainingTime => remainingTime;
    public bool GameRunning => gameRunning;

    private void Start()
    {
        score = 0;

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.Sandbox ||
            GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot)
        {
            remainingTime = 0f;
        }
        else
        {
            remainingTime = GameSessionSettings.Instance.selectedTimeLimit;
        }

        UpdateScoreUI();
        UpdateTimerUI();

        if (instructionText != null)
        {
            if (GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.Markerless)
                instructionText.text = "Scan the floor, then tap a detected plane to place the hoop.";
            else
                instructionText.text = "Look at the marker to place the hoop.";
        }
    }

    private void Update()
    {
        if (!gameRunning)
            return;

        if (gamePaused)
            return;

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.Sandbox ||
            GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0f);

        UpdateTimerUI();

        if (remainingTime <= 0f)
            EndGame();
    }

    // Used by old HoopManager/UI workflow
    public void StartGameAfterPlacement()
    {
        StartCoroutine(StartCountdownRoutine());
    }

    // Used by newer direct workflow
    public void StartGame()
    {
        StartGameAfterPlacement();
    }

    private IEnumerator StartCountdownRoutine()
    {
        if (uiManager != null)
        {
            uiManager.ShowCountdown("3");
            yield return new WaitForSeconds(1f);

            uiManager.ShowCountdown("2");
            yield return new WaitForSeconds(1f);

            uiManager.ShowCountdown("1");
            yield return new WaitForSeconds(1f);

            uiManager.ShowCountdown("GO!");
            yield return new WaitForSeconds(0.5f);

            uiManager.HideCountdown();
        }

        gameRunning = true;
        gamePaused = false;
        score = 0;

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.Sandbox ||
            GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot)
        {
            remainingTime = 0f;
        }
        else
        {
            remainingTime = GameSessionSettings.Instance.selectedTimeLimit;
        }

        UpdateScoreUI();
        UpdateTimerUI();

        if (instructionText != null)
            instructionText.text = "Click and drag the ball upward to throw.";

        if (ballSpawnManager != null)
            ballSpawnManager.SpawnBallAtSocket();
        else
            Debug.LogError("BrokenHoopsGameManager: BallSpawnManager is not assigned.");

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot &&
            trickShotChallengeManager != null)
        {
            trickShotChallengeManager.ResetTrickShotProgress();
        }
    }

    public void AddScore(int amount)
    {
        if (!gameRunning)
            return;

        score += amount;
        UpdateScoreUI();
    }

    public void PauseGameForMenu()
    {
        gamePaused = true;
    }

    public void ResumeGameFromMenu()
    {
        gamePaused = false;
    }

    private void EndGame()
    {
        gameRunning = false;

        if (instructionText != null)
            instructionText.text = "TIME'S UP";

        if (uiManager != null)
            uiManager.ShowEndGame(score);
    }

    public void Retry()
    {
        score = 0;

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.Sandbox ||
            GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot)
        {
            remainingTime = 0f;
        }
        else
        {
            remainingTime = GameSessionSettings.Instance.selectedTimeLimit;
        }

        UpdateScoreUI();
        UpdateTimerUI();

        if (uiManager != null)
            uiManager.HideEndGame();

        if (ballSpawnManager != null)
            ballSpawnManager.ClearExistingBallImmediate();

        if (trickShotChallengeManager != null)
            trickShotChallengeManager.ResetTrickShotProgress();

        StartGameAfterPlacement();
    }

    private void UpdateScoreUI()
    {
        if (uiManager != null)
            uiManager.UpdateScore(score);

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateTimerUI()
    {
        GameMode mode = GameSessionSettings.Instance.selectedGameMode;

        if (uiManager != null)
            uiManager.UpdateTimer(remainingTime, mode);

        if (timerText == null)
            return;

        if (mode == GameMode.Sandbox)
        {
            timerText.text = "Sandbox";
            return;
        }

        if (mode == GameMode.TrickShot)
        {
            timerText.text = "Trick Shot";
            return;
        }

        int seconds = Mathf.CeilToInt(remainingTime);
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        timerText.text = $"{minutes:00}:{remainingSeconds:00}";
    }
}