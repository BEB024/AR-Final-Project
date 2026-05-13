using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JukeboxManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private AudioSource audioSource;

    private int currentIndex = -1;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
            PlayRandomSong();
    }

    public void PlayRandomSong()
    {
        if (playlist == null || playlist.Length == 0)
            return;

        int nextIndex = Random.Range(0, playlist.Length);

        if (playlist.Length > 1)
        {
            while (nextIndex == currentIndex)
                nextIndex = Random.Range(0, playlist.Length);
        }

        currentIndex = nextIndex;

        audioSource.clip = playlist[currentIndex];
        audioSource.volume = GameSessionSettings.Instance.musicVolume;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}