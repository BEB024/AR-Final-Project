using UnityEngine;

public class HoopController : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private Renderer backboardRenderer;
    [SerializeField] private Renderer rimRenderer;
    [SerializeField] private Renderer netRenderer;

    [Header("Backboard Materials")]
    [SerializeField] private Material[] backboardMaterials;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip voiceoverClip;

    [Header("VFX")]
    [SerializeField] private Transform scoreVFXAnchor;
    [SerializeField] private Transform rimVFXAnchor;
    [SerializeField] private Transform backboardVFXAnchor;

    [SerializeField] private GameObject scoreSwishVFXPrefab;
    [SerializeField] private GameObject rimHitVFXPrefab;
    [SerializeField] private GameObject backboardHitVFXPrefab;

    private int backboardIndex;

    public void SetBackboardMaterialIndex(int index)
    {
        if (backboardMaterials == null || backboardMaterials.Length == 0)
            return;

        backboardIndex = Mathf.Clamp(index, 0, backboardMaterials.Length - 1);

        if (backboardRenderer != null)
            backboardRenderer.material = backboardMaterials[backboardIndex];
    }

    public void NextBackboardColor()
    {
        if (backboardMaterials == null || backboardMaterials.Length == 0)
            return;

        backboardIndex = (backboardIndex + 1) % backboardMaterials.Length;

        if (backboardRenderer != null)
            backboardRenderer.material = backboardMaterials[backboardIndex];

        GameSessionSettings.Instance.selectedBackboardColorIndex = backboardIndex;
    }

    public void PlaySpawnAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Spawn");
    }

    public void PlayScoreFeedback()
    {
        if (animator != null)
            animator.SetTrigger("Score");

        if (audioSource != null && scoreClip != null)
            audioSource.PlayOneShot(scoreClip);

        PlayScoreVFX();
    }

    public void PlayRimShake()
    {
        if (animator != null)
            animator.SetTrigger("RimShake");

        PlayRimHitVFX();
    }

    public void PlayVoiceover()
    {
        if (audioSource != null && voiceoverClip != null)
            audioSource.PlayOneShot(voiceoverClip);
    }

    public void PlayScoreVFX()
    {
        if (scoreSwishVFXPrefab != null && scoreVFXAnchor != null)
            Instantiate(scoreSwishVFXPrefab, scoreVFXAnchor.position, scoreVFXAnchor.rotation);
    }

    public void PlayRimHitVFX()
    {
        if (rimHitVFXPrefab != null && rimVFXAnchor != null)
            Instantiate(rimHitVFXPrefab, rimVFXAnchor.position, rimVFXAnchor.rotation);
    }

    public void PlayBackboardHitVFX()
    {
        if (backboardHitVFXPrefab != null && backboardVFXAnchor != null)
            Instantiate(backboardHitVFXPrefab, backboardVFXAnchor.position, backboardVFXAnchor.rotation);
    }

    public void OnBackboardTapped()
    {
        NextBackboardColor();
    }

    public void OnRimTapped()
    {
        PlayRimShake();
    }
}