using UnityEngine;
using UnityEngine.Events;

public class VehicleController : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private Renderer[] bodyRenderers;
    [SerializeField] private Renderer[] wheelRenderers;

    [Header("Door Animators")]
    [SerializeField] private Animator leftDoorAnimator;
    [SerializeField] private Animator rightDoorAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource voiceAudioSource;

    [Header("Rotation")]
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private bool autoRotate;
    [SerializeField] private float autoRotateSpeed = 15f;

    [Header("Scale Limits")]
    [SerializeField] private float minScale = 0.6f;
    [SerializeField] private float maxScale = 1.4f;

    private Material[] bodyOptions;
    private Material[] wheelOptions;
    private int bodyIndex;
    private int wheelIndex;
    private bool leftDoorOpen;
    private bool rightDoorOpen;

    public UnityEvent OnVehicleConfigured;

    public void Initialize(VehicleDefinition definition)
    {
        bodyOptions = definition.bodyMaterials;
        wheelOptions = definition.wheelMaterials;

        if (engineAudioSource != null)
            engineAudioSource.clip = definition.engineClip;

        if (voiceAudioSource != null)
            voiceAudioSource.clip = definition.voiceoverClip;

        ApplyBodyMaterial(0);
        ApplyWheelMaterial(0);
    }

    private void Update()
    {
        if (autoRotate && rotationPivot != null)
        {
            rotationPivot.Rotate(0f, autoRotateSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }

    public void NextBodyMaterial()
    {
        if (bodyOptions == null || bodyOptions.Length == 0) return;
        bodyIndex = (bodyIndex + 1) % bodyOptions.Length;
        ApplyBodyMaterial(bodyIndex);
    }

    public void NextWheelMaterial()
    {
        if (wheelOptions == null || wheelOptions.Length == 0) return;
        wheelIndex = (wheelIndex + 1) % wheelOptions.Length;
        ApplyWheelMaterial(wheelIndex);
    }

    private void ApplyBodyMaterial(int index)
    {
        if (bodyOptions == null || bodyOptions.Length == 0) return;

        foreach (var rend in bodyRenderers)
        {
            rend.material = bodyOptions[index];
        }
    }

    private void ApplyWheelMaterial(int index)
    {
        if (wheelOptions == null || wheelOptions.Length == 0) return;

        foreach (var rend in wheelRenderers)
        {
            rend.material = wheelOptions[index];
        }
    }

    public void ToggleEngine()
    {
        if (engineAudioSource == null || engineAudioSource.clip == null) return;

        if (engineAudioSource.isPlaying)
            engineAudioSource.Stop();
        else
            engineAudioSource.Play();
    }

    public void PlayVoiceover()
    {
        if (voiceAudioSource == null || voiceAudioSource.clip == null) return;

        if (voiceAudioSource.isPlaying)
            voiceAudioSource.Stop();

        voiceAudioSource.Play();
    }

    public void ToggleLeftDoor()
    {
        leftDoorOpen = !leftDoorOpen;
        if (leftDoorAnimator != null)
            leftDoorAnimator.SetBool("Open", leftDoorOpen);
    }

    public void ToggleRightDoor()
    {
        rightDoorOpen = !rightDoorOpen;
        if (rightDoorAnimator != null)
            rightDoorAnimator.SetBool("Open", rightDoorOpen);
    }

    public void SetAutoRotate(bool enabled)
    {
        autoRotate = enabled;
    }

    public void SetRotateSpeed(float speed)
    {
        autoRotateSpeed = speed;
    }

    public void RotateManually(float yawAmount)
    {
        if (rotationPivot != null)
            rotationPivot.Rotate(0f, yawAmount, 0f, Space.World);
    }

    public void RotateTwoFinger(float yawAmount)
    {
        if (rotationPivot != null)
            rotationPivot.Rotate(0f, yawAmount, 0f, Space.World);
    }

    public void ScaleByDelta(float delta)
    {
        Vector3 current = transform.localScale;
        float target = Mathf.Clamp(current.x + delta, minScale, maxScale);
        transform.localScale = new Vector3(target, target, target);
    }
}