using UnityEngine;
// using UnityEngine.SceneManagement;
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

    [Header("AR Placement Objects")]
    [SerializeField] private GameObject markerlessPlacementObject;
    [SerializeField] private GameObject markerBasedTrackerObject;

    [Header("Customization Names")]
    [SerializeField] private string[] ballNames;
    [SerializeField] private string[] colorNames;

    [Header("End Game Score Images")]
    [SerializeField] private GameObject flightImageBad;
    [SerializeField] private GameObject flightImageOK;
    [SerializeField] private GameObject flightImageGood;
    [SerializeField] private GameObject flightImageGreat;
    [SerializeField] private GameObject flightImageAwesome;

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

        if (challengePanel != null && GameSessionSettings.Instance != null)
            challengePanel.SetActive(GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot);

        ApplySpawnModeObjects();
        UpdateInstructionText();
        UpdateCustomizationLabels();
        UpdateSettingsLabels();
    }

    public void UpdateInstructionText()
    {
        if (instructionText == null || GameSessionSettings.Instance == null)
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

        UpdateEndGameScoreImage(finalScore);
    }

    public void HideEndGame()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        HideAllEndGameScoreImages();
    }

    private void UpdateEndGameScoreImage(int finalScore)
    {
        HideAllEndGameScoreImages();

        if (finalScore <= 0)
        {
            if (flightImageBad != null)
                flightImageBad.SetActive(true);

            return;
        }

        if (finalScore >= 1 && finalScore <= 5)
        {
            if (flightImageOK != null)
                flightImageOK.SetActive(true);

            return;
        }

        if (finalScore >= 6 && finalScore <= 10)
        {
            if (flightImageGood != null)
                flightImageGood.SetActive(true);

            return;
        }

        if (finalScore >= 11 && finalScore <= 15)
        {
            if (flightImageGreat != null)
                flightImageGreat.SetActive(true);

            return;
        }

        if (finalScore >= 16)
        {
            if (flightImageAwesome != null)
                flightImageAwesome.SetActive(true);
        }
    }

    private void HideAllEndGameScoreImages()
    {
        if (flightImageBad != null)
            flightImageBad.SetActive(false);

        if (flightImageOK != null)
            flightImageOK.SetActive(false);

        if (flightImageGood != null)
            flightImageGood.SetActive(false);

        if (flightImageGreat != null)
            flightImageGreat.SetActive(false);

        if (flightImageAwesome != null)
            flightImageAwesome.SetActive(false);
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
        if (gameManager != null)
            gameManager.PauseGameForMenu();

        HideAllPopups();

        if (basketballSelectionPanel != null)
            basketballSelectionPanel.SetActive(true);

        UpdateCustomizationLabels();
    }

    public void CloseBasketballSelection()
    {
        if (basketballSelectionPanel != null)
            basketballSelectionPanel.SetActive(false);

        if (gameManager != null)
            gameManager.ResumeGameFromMenu();
    }

    public void NextBall()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (ballNames == null || ballNames.Length == 0)
            return;

        GameSessionSettings.Instance.selectedBallIndex++;

        if (GameSessionSettings.Instance.selectedBallIndex >= ballNames.Length)
            GameSessionSettings.Instance.selectedBallIndex = 0;

        if (ballSpawnManager != null)
            ballSpawnManager.SpawnBallAtSocket();

        UpdateCustomizationLabels();
    }

    public void PreviousBall()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (ballNames == null || ballNames.Length == 0)
            return;

        GameSessionSettings.Instance.selectedBallIndex--;

        if (GameSessionSettings.Instance.selectedBallIndex < 0)
            GameSessionSettings.Instance.selectedBallIndex = ballNames.Length - 1;

        if (ballSpawnManager != null)
            ballSpawnManager.SpawnBallAtSocket();

        UpdateCustomizationLabels();
    }

    public void OpenBackboardColor()
    {
        if (gameManager != null)
            gameManager.PauseGameForMenu();

        HideAllPopups();

        if (backboardColorPanel != null)
            backboardColorPanel.SetActive(true);

        UpdateCustomizationLabels();
    }

    public void CloseBackboardColor()
    {
        if (backboardColorPanel != null)
            backboardColorPanel.SetActive(false);

        if (gameManager != null)
            gameManager.ResumeGameFromMenu();
    }

    public void NextBackboardColor()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (colorNames == null || colorNames.Length == 0)
            return;

        GameSessionSettings.Instance.selectedBackboardColorIndex++;

        if (GameSessionSettings.Instance.selectedBackboardColorIndex >= colorNames.Length)
            GameSessionSettings.Instance.selectedBackboardColorIndex = 0;

        if (hoopManager != null && hoopManager.ActiveHoopController != null)
        {
            hoopManager.ActiveHoopController.SetBackboardMaterialIndex(
                GameSessionSettings.Instance.selectedBackboardColorIndex
            );
        }

        UpdateCustomizationLabels();
    }

    public void PreviousBackboardColor()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (colorNames == null || colorNames.Length == 0)
            return;

        GameSessionSettings.Instance.selectedBackboardColorIndex--;

        if (GameSessionSettings.Instance.selectedBackboardColorIndex < 0)
            GameSessionSettings.Instance.selectedBackboardColorIndex = colorNames.Length - 1;

        if (hoopManager != null && hoopManager.ActiveHoopController != null)
        {
            hoopManager.ActiveHoopController.SetBackboardMaterialIndex(
                GameSessionSettings.Instance.selectedBackboardColorIndex
            );
        }

        UpdateCustomizationLabels();
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null)
            return;

        bool show = !settingsPanel.activeSelf;

        HideAllPopups();
        settingsPanel.SetActive(show);

        if (gameManager != null)
        {
            if (show)
                gameManager.PauseGameForMenu();
            else
                gameManager.ResumeGameFromMenu();
        }

        UpdateSettingsLabels();
    }

    public void ToggleSocketMode()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
            GameSessionSettings.Instance.socketMode = BallSocketMode.ManualPlacement;
        else
            GameSessionSettings.Instance.socketMode = BallSocketMode.AutoSocket;

        UpdateSettingsLabels();
    }

    public void ToggleSpawnMode()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.MarkerBased)
            GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.Markerless;
        else
            GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.MarkerBased;

        if (hoopManager != null)
            hoopManager.ClearHoop();

        if (ballSpawnManager != null)
            ballSpawnManager.ClearExistingBallImmediate();

        ApplySpawnModeObjects();
        UpdateSettingsLabels();
        UpdateInstructionText();
    }

    private void ApplySpawnModeObjects()
    {
        if (GameSessionSettings.Instance == null)
            return;

        bool markerless = GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.Markerless;
        bool markerBased = GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.MarkerBased;

        if (markerlessPlacementObject != null)
            markerlessPlacementObject.SetActive(markerless);

        if (markerBasedTrackerObject != null)
            markerBasedTrackerObject.SetActive(markerBased);
    }

    public void SetThrowSensitivity(float value)
    {
        if (GameSessionSettings.Instance != null)
            GameSessionSettings.Instance.throwSensitivity = value;
    }

    public void SetVolume(float value)
    {
        if (GameSessionSettings.Instance != null)
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
        if (AppSceneManager.Instance != null)
            AppSceneManager.Instance.RestartGame();
    }

    public void ExitToMainMenu()
    {
        if (AppSceneManager.Instance != null)
            AppSceneManager.Instance.LoadMainMenu();
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
        if (GameSessionSettings.Instance == null)
            return;

        if (ballNames != null && ballNames.Length > 0 && ballNameText != null)
        {
            int ballIndex = Mathf.Clamp(GameSessionSettings.Instance.selectedBallIndex, 0, ballNames.Length - 1);
            GameSessionSettings.Instance.selectedBallIndex = ballIndex;
            ballNameText.text = ballNames[ballIndex];
        }

        if (colorNames != null && colorNames.Length > 0 && colorNameText != null)
        {
            int colorIndex = Mathf.Clamp(GameSessionSettings.Instance.selectedBackboardColorIndex, 0, colorNames.Length - 1);
            GameSessionSettings.Instance.selectedBackboardColorIndex = colorIndex;
            colorNameText.text = colorNames[colorIndex];
        }
    }

    private void UpdateSettingsLabels()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (socketToggleText != null)
            socketToggleText.text = "Socket: " + GameSessionSettings.Instance.socketMode;

        if (spawnModeToggleText != null)
            spawnModeToggleText.text = "Spawn: " + GameSessionSettings.Instance.selectedSpawnMode;
    }
}