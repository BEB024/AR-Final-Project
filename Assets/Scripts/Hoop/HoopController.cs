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

    private int backboardIndex;

    public void SetBackboardMaterialIndex(int index)
    {
        if (backboardMaterials == null || backboardMaterials.Length == 0) return;

        backboardIndex = Mathf.Clamp(index, 0, backboardMaterials.Length - 1);
        backboardRenderer.material = backboardMaterials[backboardIndex];
    }

    public void NextBackboardColor()
    {
        if (backboardMaterials == null || backboardMaterials.Length == 0) return;

        backboardIndex = (backboardIndex + 1) % backboardMaterials.Length;
        backboardRenderer.material = backboardMaterials[backboardIndex];

        GameSessionSettings.Instance.selectedBackboardColorIndex = backboardIndex;
    }

    public void PlayScoreFeedback()
    {
        if (animator != null)
            animator.SetTrigger("Score");

        if (audioSource != null && scoreClip != null)
            audioSource.PlayOneShot(scoreClip);
    }

    public void PlayVoiceover()
    {
        if (audioSource != null && voiceoverClip != null)
            audioSource.PlayOneShot(voiceoverClip);
    }

    public void OnBackboardTapped()
    {
        NextBackboardColor();
    }

    public void OnRimTapped()
    {
        if (animator != null)
            animator.SetTrigger("RimShake");
    }
}
