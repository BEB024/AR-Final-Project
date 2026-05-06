using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Time Trial Dropdown")]
    [SerializeField] private TMP_Dropdown timeDropdown;

    private void Start()
    {
        if (timeDropdown != null)
        {
            timeDropdown.ClearOptions();
            timeDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "10 seconds",
                "30 seconds",
                "1 minute",
                "2 minutes",
                "3 minutes",
                "5 minutes"
            });

            timeDropdown.value = 2;
            ApplyTimeDropdown();
        }
    }

    public void SelectSandbox()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.Sandbox;
    }

    public void SelectTimeTrial()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.TimeTrial;
    }

    public void SelectFlightStyle()
    {
        GameSessionSettings.Instance.selectedGameMode = GameMode.FlightStyle;
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
        if (timeDropdown == null) return;

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

    public void StartGame()
    {
        ApplyTimeDropdown();
        SceneManager.LoadScene("AR_Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
