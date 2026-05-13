using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject basketballSelectionPanel;
    [SerializeField] private GameObject backboardColorPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Text")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text ballNameText;
    [SerializeField] private TMP_Text colorNameText;
    [SerializeField] private TMP_Text socketToggleText;
    [SerializeField] private TMP_Text spawnModeToggleText;

    [Header("Trick Shot UI")]
    [SerializeField] private GameObject challengePanel;
    [SerializeField] private TMP_Text challengeTitleText;
    [SerializeField] private TMP_Text challengeDescriptionText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text challengeResultText;

    [Header("References")]
    [SerializeField] private BrokenHoopsGameManager gameManager;
    [SerializeField] private BallSpawnManager ballSpawnManager;
    [SerializeField] private HoopManager hoopManager;
    [SerializeField] private JukeboxManager jukeboxManager;
    [SerializeField] private ARModeActivator arModeActivator;

    [Header("Customization Names")]
    [SerializeField] private string[] ballNames;
    [SerializeField] private string[] colorNames;

    private void Start()
    {
        HideAllPopups();

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        if (setupPanel != null)
            setupPanel.SetActive(true);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        if (challengePanel != null)
            challengePanel.SetActive(GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot);

        UpdateInstructionText();
        UpdateCustomizationLabels();
        UpdateSettingsLabels();
    }

    public void UpdateInstructionText()
    {
        if (instructionText == null)
            return;

        if (GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.MarkerBased)
            instructionText.text = "Point the camera at the basketball marker to spawn the hoop.";
        else
            instructionText.text = "Scan the floor, then tap a detected plane to place the hoop.";
    }

    public void ShowPlacementConfirmPanel()
    {
        if (setupPanel != null)
            setupPanel.SetActive(true);

        if (instructionText != null)
            instructionText.text = "Hoop placed. Press Confirm to start.";
    }

    public void HidePlacementConfirmPanel()
    {
        if (setupPanel != null)
            setupPanel.SetActive(false);
    }

    public void ConfirmPlacement()
    {
        if (hoopManager != null)
            hoopManager.ConfirmHoopPlacement();
    }

    public void ShowCountdown(string value)
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        if (countdownText != null)
            countdownText.text = value;
    }

    public void HideCountdown()
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void UpdateTimer(float time, GameMode mode)
    {
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

        int seconds = Mathf.CeilToInt(time);
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        timerText.text = $"{minutes:00}:{remainingSeconds:00}";
    }

    public void ShowEndGame(int finalScore)
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;
    }

    public void HideEndGame()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    public void ShowChallenge(string title, string description, int combo)
    {
        if (challengePanel != null)
            challengePanel.SetActive(true);

        if (challengeTitleText != null)
            challengeTitleText.text = title;

        if (challengeDescriptionText != null)
            challengeDescriptionText.text = description;

        if (comboText != null)
            comboText.text = "Combo: x" + Mathf.Max(1, combo);

        if (challengeResultText != null)
            challengeResultText.text = "";
    }

    public void ShowChallengeResult(string result, int combo)
    {
        if (challengeResultText != null)
            challengeResultText.text = result;

        if (comboText != null)
            comboText.text = "Combo: x" + Mathf.Max(1, combo);
    }

    public void OpenBasketballSelection()
    {
        gameManager.PauseGameForMenu();
        HideAllPopups();
        basketballSelectionPanel.SetActive(true);
        UpdateCustomizationLabels();
    }

    public void CloseBasketballSelection()
    {
        basketballSelectionPanel.SetActive(false);
        gameManager.ResumeGameFromMenu();
    }

    public void NextBall()
    {
        GameSessionSettings.Instance.selectedBallIndex++;

        if (GameSessionSettings.Instance.selectedBallIndex >= ballNames.Length)
            GameSessionSettings.Instance.selectedBallIndex = 0;

        ballSpawnManager.SpawnBallAtSocket();
        UpdateCustomizationLabels();
    }

    public void PreviousBall()
    {
        GameSessionSettings.Instance.selectedBallIndex--;

        if (GameSessionSettings.Instance.selectedBallIndex < 0)
            GameSessionSettings.Instance.selectedBallIndex = ballNames.Length - 1;

        ballSpawnManager.SpawnBallAtSocket();
        UpdateCustomizationLabels();
    }

    public void OpenBackboardColor()
    {
        gameManager.PauseGameForMenu();
        HideAllPopups();
        backboardColorPanel.SetActive(true);
        UpdateCustomizationLabels();
    }

    public void CloseBackboardColor()
    {
        backboardColorPanel.SetActive(false);
        gameManager.ResumeGameFromMenu();
    }

    public void NextBackboardColor()
    {
        GameSessionSettings.Instance.selectedBackboardColorIndex++;

        if (GameSessionSettings.Instance.selectedBackboardColorIndex >= colorNames.Length)
            GameSessionSettings.Instance.selectedBackboardColorIndex = 0;

        if (hoopManager.ActiveHoopController != null)
            hoopManager.ActiveHoopController.SetBackboardMaterialIndex(GameSessionSettings.Instance.selectedBackboardColorIndex);

        UpdateCustomizationLabels();
    }

    public void PreviousBackboardColor()
    {
        GameSessionSettings.Instance.selectedBackboardColorIndex--;

        if (GameSessionSettings.Instance.selectedBackboardColorIndex < 0)
            GameSessionSettings.Instance.selectedBackboardColorIndex = colorNames.Length - 1;

        if (hoopManager.ActiveHoopController != null)
            hoopManager.ActiveHoopController.SetBackboardMaterialIndex(GameSessionSettings.Instance.selectedBackboardColorIndex);

        UpdateCustomizationLabels();
    }

    public void ToggleSettings()
    {
        bool show = !settingsPanel.activeSelf;

        HideAllPopups();
        settingsPanel.SetActive(show);

        if (show)
            gameManager.PauseGameForMenu();
        else
            gameManager.ResumeGameFromMenu();

        UpdateSettingsLabels();
    }

    public void ToggleSocketMode()
    {
        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
            GameSessionSettings.Instance.socketMode = BallSocketMode.ManualPlacement;
        else
            GameSessionSettings.Instance.socketMode = BallSocketMode.AutoSocket;

        UpdateSettingsLabels();
    }

    public void ToggleSpawnMode()
    {
        if (GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.MarkerBased)
            GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.Markerless;
        else
            GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.MarkerBased;

        if (hoopManager != null)
            hoopManager.ClearHoop();

        if (ballSpawnManager != null)
            ballSpawnManager.ClearExistingBallImmediate();

        if (arModeActivator != null)
            arModeActivator.ApplyMode();

        UpdateSettingsLabels();
        UpdateInstructionText();
    }

    public void SetThrowSensitivity(float value)
    {
        GameSessionSettings.Instance.throwSensitivity = value;
    }

    public void SetVolume(float value)
    {
        GameSessionSettings.Instance.musicVolume = value;
        AudioListener.volume = value;
    }

    public void ManualSpawnBall()
    {
        if (ballSpawnManager != null)
            ballSpawnManager.ManualSpawnInFrontOfCamera();
    }

    public void PlayRandomMusic()
    {
        if (jukeboxManager != null)
            jukeboxManager.PlayRandomSong();
    }

    public void Retry()
    {
        gameManager.Retry();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void HideAllPopups()
    {
        if (basketballSelectionPanel != null)
            basketballSelectionPanel.SetActive(false);

        if (backboardColorPanel != null)
            backboardColorPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void UpdateCustomizationLabels()
    {
        if (ballNames != null && ballNames.Length > 0 && ballNameText != null)
            ballNameText.text = ballNames[GameSessionSettings.Instance.selectedBallIndex];

        if (colorNames != null && colorNames.Length > 0 && colorNameText != null)
            colorNameText.text = colorNames[GameSessionSettings.Instance.selectedBackboardColorIndex];
    }

    private void UpdateSettingsLabels()
    {
        if (socketToggleText != null)
            socketToggleText.text = "Socket: " + GameSessionSettings.Instance.socketMode;

        if (spawnModeToggleText != null)
            spawnModeToggleText.text = "Spawn: " + GameSessionSettings.Instance.selectedSpawnMode;
    }
}