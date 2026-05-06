using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketballController : MonoBehaviour
{
    [Header("State")]
    public bool IsReleased { get; private set; }
    public bool HasScored { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField] private AudioClip bounceClip;
    [SerializeField] private AudioClip releaseClip;

    private BallSpawnManager spawnManager;
    private Coroutine missedShotCoroutine;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public void Initialize(BallSpawnManager manager)
    {
        spawnManager = manager;
        ResetBallPhysics();
    }

    public void ResetBallPhysics()
    {
        IsReleased = false;
        HasScored = false;

        if (missedShotCoroutine != null)
        {
            StopCoroutine(missedShotCoroutine);
            missedShotCoroutine = null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        if (trailRenderer != null)
            trailRenderer.Clear();
    }

    public void ReleaseBall(Vector3 force, bool flightStyle)
    {
        IsReleased = true;
        HasScored = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        if (flightStyle)
            force = ApplyFlightStyleRandomness(force);

        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

        if (audioSource != null && releaseClip != null)
            audioSource.PlayOneShot(releaseClip);

        missedShotCoroutine = StartCoroutine(MissedShotLifeTimer());
    }

    private Vector3 ApplyFlightStyleRandomness(Vector3 originalForce)
    {
        float powerMultiplier = Random.Range(0.55f, 1.65f);
        float sideSwerve = Random.Range(-1.4f, 1.4f);
        float verticalChaos = Random.Range(-0.25f, 0.45f);

        Vector3 randomized = originalForce * powerMultiplier;
        randomized += Camera.main.transform.right * sideSwerve;
        randomized += Vector3.up * verticalChaos;

        return randomized;
    }

    private IEnumerator MissedShotLifeTimer()
    {
        yield return new WaitForSeconds(5f);

        if (!HasScored && spawnManager != null)
            spawnManager.HandleBallMissedInsideRadius(this);
    }

    public void MarkScored()
    {
        if (HasScored) return;

        HasScored = true;

        if (missedShotCoroutine != null)
        {
            StopCoroutine(missedShotCoroutine);
            missedShotCoroutine = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource != null && bounceClip != null && collision.relativeVelocity.magnitude > 1f)
            audioSource.PlayOneShot(bounceClip, 0.5f);
    }
}