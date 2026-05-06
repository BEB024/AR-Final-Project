using UnityEngine;

public class GameSessionSettings : MonoBehaviour
{
    public static GameSessionSettings Instance { get; private set; }

    [Header("Selected Mode")]
    public GameMode selectedGameMode = GameMode.Sandbox;
    public SpawnMode selectedSpawnMode = SpawnMode.MarkerBased;

    [Header("Time Trial")]
    public float selectedTimeLimit = 60f;

    [Header("Settings")]
    public BallSocketMode socketMode = BallSocketMode.AutoSocket;
    public float throwSensitivity = 1.2f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 1.0f;

    [Header("Customization")]
    public int selectedBallIndex = 0;
    public int selectedBackboardColorIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}