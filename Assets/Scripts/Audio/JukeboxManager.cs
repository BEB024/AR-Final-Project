using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class JukeboxManager : MonoBehaviour
{
    [Header("Playlists")]
    [FormerlySerializedAs("playlist")]
    [SerializeField] private AudioClip[] standardPlaylist;

    [SerializeField] private AudioClip[] flightStylePlaylist;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private int currentIndex = -1;
    private bool usingFlightPlaylist;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayRandomSong();
    }

    private void Update()
    {
        if (audioSource == null)
            return;

        if (audioSource.clip != null && !audioSource.isPlaying)
            PlayRandomSong();
    }

    public void PlayRandomSong()
    {
        if (audioSource == null)
            return;

        AudioClip[] activePlaylist = GetActivePlaylist();

        if (activePlaylist == null || activePlaylist.Length == 0)
        {
            Debug.LogWarning("JukeboxManager: No songs assigned for the current game mode.");
            return;
        }

        int nextIndex = Random.Range(0, activePlaylist.Length);

        if (activePlaylist.Length > 1)
        {
            while (nextIndex == currentIndex)
                nextIndex = Random.Range(0, activePlaylist.Length);
        }

        currentIndex = nextIndex;

        audioSource.clip = activePlaylist[currentIndex];

        if (GameSessionSettings.Instance != null)
            audioSource.volume = GameSessionSettings.Instance.musicVolume;

        audioSource.Play();
    }

    private AudioClip[] GetActivePlaylist()
    {
        bool shouldUseFlightPlaylist =
            GameSessionSettings.Instance != null &&
            GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;

        if (shouldUseFlightPlaylist != usingFlightPlaylist)
        {
            usingFlightPlaylist = shouldUseFlightPlaylist;
            currentIndex = -1;
        }

        if (usingFlightPlaylist)
            return flightStylePlaylist;

        return standardPlaylist;
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}

// using UnityEngine;

// [RequireComponent(typeof(AudioSource))]
// public class JukeboxManager : MonoBehaviour
// {
//     [SerializeField] private AudioClip[] playlist;
//     [SerializeField] private AudioSource audioSource;

//     private int currentIndex = -1;

//     private void Awake()
//     {
//         if (audioSource == null)
//             audioSource = GetComponent<AudioSource>();
//     }

//     private void Update()
//     {
//         if (audioSource.clip != null && !audioSource.isPlaying)
//             PlayRandomSong();
//     }

//     public void PlayRandomSong()
//     {
//         if (playlist == null || playlist.Length == 0)
//             return;

//         int nextIndex = Random.Range(0, playlist.Length);

//         if (playlist.Length > 1)
//         {
//             while (nextIndex == currentIndex)
//                 nextIndex = Random.Range(0, playlist.Length);
//         }

//         currentIndex = nextIndex;

//         audioSource.clip = playlist[currentIndex];
//         audioSource.volume = GameSessionSettings.Instance.musicVolume;
//         audioSource.Play();
//     }

//     public void StopMusic()
//     {
//         audioSource.Stop();
//     }
// }