using System.Collections;
using UnityEngine;

public class BrokenHoopsGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallSpawnManager ballSpawnManager;
    [SerializeField] private GameplayUIManager uiManager;
    [SerializeField] private TrickShotChallengeManager trickShotChallengeManager;

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

        uiManager.UpdateScore(score);
        uiManager.UpdateTimer(remainingTime, GameSessionSettings.Instance.selectedGameMode);
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

        uiManager.UpdateTimer(remainingTime, GameSessionSettings.Instance.selectedGameMode);

        if (remainingTime <= 0f)
            EndGame();
    }

    public void StartGameAfterPlacement()
    {
        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
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

        gameRunning = true;
        score = 0;

        uiManager.UpdateScore(score);
        ballSpawnManager.SpawnBallAtSocket();

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
        uiManager.UpdateScore(score);
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
        uiManager.ShowEndGame(score);
    }

    public void Retry()
    {
        score = 0;

        if (GameSessionSettings.Instance.selectedGameMode != GameMode.Sandbox &&
            GameSessionSettings.Instance.selectedGameMode != GameMode.TrickShot)
        {
            remainingTime = GameSessionSettings.Instance.selectedTimeLimit;
        }

        uiManager.UpdateScore(score);
        uiManager.UpdateTimer(remainingTime, GameSessionSettings.Instance.selectedGameMode);
        uiManager.HideEndGame();

        ballSpawnManager.ClearExistingBallImmediate();

        if (trickShotChallengeManager != null)
            trickShotChallengeManager.ResetTrickShotProgress();

        StartGameAfterPlacement();
    }
}