using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Existing Time Dropdown")]
    [SerializeField] private TMP_Dropdown timeDropdown;

    [Header("Panels")]
    [SerializeField] private GameObject defaultPanel;
    [SerializeField] private GameObject modePanel;
    [SerializeField] private GameObject trialsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutPanel;

    [Header("Settings")]
    [SerializeField] private Slider volumeSlider;

    [Header("About Text")]
    [SerializeField] private TMP_Text madeByText;
    [SerializeField] private TMP_Text projectInfoText;

    [Header("Main Menu Basketball Icon")]
    [SerializeField] private Transform mainMenuBasketball;
    [SerializeField] private float basketballRotationSpeed = 90f;
    [SerializeField] private bool rotateBasketballOnZAxis = true;

    [TextArea]
    [SerializeField] private string madeByValue = "Made by: Your Name";

    [TextArea]
    [SerializeField] private string projectInfoValue = "Broken Hoops\nAR Final Project\nUnity 6 / AR Foundation";

    private void Start()
    {
        SetDefaultGameSettings();
        SetupTimeDropdown();
        SetupVolumeSlider();
        SetupAboutText();
        ShowDefaultPanel();
    }

    private void Update()
    {
        RotateMainMenuBasketball();
    }

    private void SetDefaultGameSettings()
    {
        if (GameSessionSettings.Instance == null)
            return;

        GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.Markerless;
        GameSessionSettings.Instance.selectedGameMode = GameMode.TimeTrial;
        GameSessionSettings.Instance.selectedTimeLimit = 60f;
    }

    private void SetupTimeDropdown()
    {
        if (timeDropdown == null)
            return;

        timeDropdown.ClearOptions();

        timeDropdown.AddOptions(new List<string>
        {
            "10 seconds",
            "30 seconds",
            "1 minute",
            "2 minutes",
            "3 minutes",
            "5 minutes"
        });

        timeDropdown.value = 2;
        timeDropdown.RefreshShownValue();
        ApplyTimeDropdown();
    }

    private void SetupVolumeSlider()
    {
        if (volumeSlider == null)
            return;

        float startVolume = 1f;

        if (GameSessionSettings.Instance != null)
            startVolume = GameSessionSettings.Instance.musicVolume;
        else
            startVolume = AudioListener.volume;

        volumeSlider.value = startVolume;

        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);

        SetVolume(startVolume);
    }

    private void SetupAboutText()
    {
        if (madeByText != null)
            madeByText.text = madeByValue;

        if (projectInfoText != null)
            projectInfoText.text = projectInfoValue;
    }

    private void HideAllPanels()
    {
        // if (defaultPanel != null)
        //     defaultPanel.SetActive(false);

        if (modePanel != null)
            modePanel.SetActive(false);

        if (trialsPanel != null)
            trialsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }

    public void ShowDefaultPanel()
    {
        HideAllPanels();

        if (defaultPanel != null)
            defaultPanel.SetActive(true);
    }

    public void ShowModePanel()
    {
        HideAllPanels();

        if (modePanel != null)
            modePanel.SetActive(true);
    }

    public void ShowTrialsPanel()
    {
        HideAllPanels();

        if (trialsPanel != null)
            trialsPanel.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        HideAllPanels();

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ShowAboutPanel()
    {
        HideAllPanels();

        if (aboutPanel != null)
            aboutPanel.SetActive(true);
    }

    public void SelectSandbox()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.Sandbox;
    }

    public void SelectTimeTrial()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.TimeTrial;
        ApplyTimeDropdown();
    }

    public void SelectFlightStyle()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.FlightStyle;
        ApplyTimeDropdown();
    }

    public void SelectTrickShot()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.TrickShot;
    }

    public void SelectMarkerBased()
    {
        GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.MarkerBased;
    }

    public void SelectMarkerless()
    {
        GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.Markerless;
    }

    public void ApplyTimeDropdown()
    {
        if (timeDropdown == null)
            return;

        switch (timeDropdown.value)
        {
            case 0:
                GameSessionSettings.Instance.selectedTimeLimit = 10f;
                break;
            case 1:
                GameSessionSettings.Instance.selectedTimeLimit = 30f;
                break;
            case 2:
                GameSessionSettings.Instance.selectedTimeLimit = 60f;
                break;
            case 3:
                GameSessionSettings.Instance.selectedTimeLimit = 120f;
                break;
            case 4:
                GameSessionSettings.Instance.selectedTimeLimit = 180f;
                break;
            case 5:
                GameSessionSettings.Instance.selectedTimeLimit = 300f;
                break;
        }
    }

    private void RotateMainMenuBasketball()
    {
        if (mainMenuBasketball == null)
            return;

        if (rotateBasketballOnZAxis)
        {
            // Best for UI Image / 2D icon rotation
            mainMenuBasketball.Rotate(0f, 0f, -basketballRotationSpeed * Time.deltaTime);
        }
        else
        {
            // Best for a 3D basketball model
            mainMenuBasketball.Rotate(0f, basketballRotationSpeed * Time.deltaTime, 0f);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;

        if (GameSessionSettings.Instance != null)
            GameSessionSettings.Instance.musicVolume = value;
    }

    public void StartGame()
    {
        ApplyTimeDropdown();

        if (GameSessionSettings.Instance != null)
            GameSessionSettings.Instance.ResetRuntimeValuesForNewGame();

        if (AppSceneManager.Instance != null)
            AppSceneManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
        if (AppSceneManager.Instance != null)
            AppSceneManager.Instance.QuitGame();
        else
            Application.Quit();
    }
}

// using UnityEngine;
// // using UnityEngine.SceneManagement;
// using TMPro;
// using System.Collections.Generic;

// public class MainMenuManager : MonoBehaviour
// {
//     [SerializeField] private TMP_Dropdown timeDropdown;

//     private void Start()
//     {
//         if (timeDropdown != null)
//         {
//             timeDropdown.ClearOptions();

//             timeDropdown.AddOptions(new List<string>
//             {
//                 "10 seconds",
//                 "30 seconds",
//                 "1 minute",
//                 "2 minutes",
//                 "3 minutes",
//                 "5 minutes"
//             });

//             timeDropdown.value = 2;
//             ApplyTimeDropdown();
//         }
//     }

//     public void SelectSandbox()
//     {
//         GameSessionSettings.Instance.selectedGameMode = GameMode.Sandbox;
//     }

//     public void SelectTimeTrial()
//     {
//         GameSessionSettings.Instance.selectedGameMode = GameMode.TimeTrial;
//     }

//     public void SelectFlightStyle()
//     {
//         GameSessionSettings.Instance.selectedGameMode = GameMode.FlightStyle;
//     }

//     public void SelectTrickShot()
//     {
//         GameSessionSettings.Instance.selectedGameMode = GameMode.TrickShot;
//     }

//     public void SelectMarkerBased()
//     {
//         GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.MarkerBased;
//     }

//     public void SelectMarkerless()
//     {
//         GameSessionSettings.Instance.selectedSpawnMode = SpawnMode.Markerless;
//     }

//     public void ApplyTimeDropdown()
//     {
//         if (timeDropdown == null)
//             return;

//         switch (timeDropdown.value)
//         {
//             case 0:
//                 GameSessionSettings.Instance.selectedTimeLimit = 10f;
//                 break;
//             case 1:
//                 GameSessionSettings.Instance.selectedTimeLimit = 30f;
//                 break;
//             case 2:
//                 GameSessionSettings.Instance.selectedTimeLimit = 60f;
//                 break;
//             case 3:
//                 GameSessionSettings.Instance.selectedTimeLimit = 120f;
//                 break;
//             case 4:
//                 GameSessionSettings.Instance.selectedTimeLimit = 180f;
//                 break;
//             case 5:
//                 GameSessionSettings.Instance.selectedTimeLimit = 300f;
//                 break;
//         }
//     }

//     public void StartGame()
//     {
//         ApplyTimeDropdown();

//         if (GameSessionSettings.Instance != null)
//             GameSessionSettings.Instance.ResetRuntimeValuesForNewGame();

//         if (AppSceneManager.Instance != null)
//             AppSceneManager.Instance.LoadGame();
//     }

//     public void QuitGame()
//     {
//         if (AppSceneManager.Instance != null)
//             AppSceneManager.Instance.QuitGame();
//         else
//             Application.Quit();
//     }

// }