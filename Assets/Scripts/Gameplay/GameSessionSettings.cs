using UnityEngine;

public class GameSessionSettings : MonoBehaviour
{
    public static GameSessionSettings Instance { get; private set; }

    [Header("Selected Mode")]
    public GameMode selectedGameMode = GameMode.Sandbox;

    [Header("Placement Mode")]
    public SpawnMode selectedSpawnMode = SpawnMode.Markerless;

    [Header("Time Trial")]
    public float selectedTimeLimit = 60f;

    [Header("Settings")]
    public BallSocketMode socketMode = BallSocketMode.AutoSocket;
    public float throwSensitivity = 0.2f;
    public float musicVolume = 0.7f;
    public float sfxVolume = 1f;

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

    public void ResetRuntimeValuesForNewGame()
    {
        socketMode = BallSocketMode.AutoSocket;
        selectedBallIndex = Mathf.Max(0, selectedBallIndex);
        selectedBackboardColorIndex = Mathf.Max(0, selectedBackboardColorIndex);
    }
    
}

// using UnityEngine;

// public class GameSessionSettings : MonoBehaviour
// {
//     public static GameSessionSettings Instance { get; private set; }

//     [Header("Mode")]
//     public GameMode selectedGameMode = GameMode.TimeTrial;
//     public SpawnMode selectedSpawnMode = SpawnMode.Markerless;

//     [Header("Time Trial")]
//     public float selectedTimeLimit = 60f;

//     [Header("Ball / Controls")]
//     public BallSocketMode socketMode = BallSocketMode.AutoSocket;
//     public float throwSensitivity = 1.2f;
//     public int selectedBallIndex = 0;

//     [Header("Audio")]
//     public float musicVolume = 0.7f;
//     public float sfxVolume = 1f;

//     [Header("Customization")]
//     public int selectedBackboardColorIndex = 0;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;

//         // Keep this if you move from MainMenu to AR_Game.
//         // If testing AR_Game directly, it is also fine.
//         DontDestroyOnLoad(gameObject);
//     }
// }

