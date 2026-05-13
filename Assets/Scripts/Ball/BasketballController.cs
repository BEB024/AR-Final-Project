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

    [Header("VFX")]
    [SerializeField] private GameObject perfectReleaseVFXPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip bounceClip;
    [SerializeField] private AudioClip releaseClip;

    [Header("No Movement Challenge")]
    [SerializeField] private float movementThreshold = 0.25f;

    private BallSpawnManager spawnManager;
    private HoopManager hoopManager;
    private Coroutine missedShotCoroutine;

    private ShotData currentShotData;
    private Vector3 playerPositionAtRelease;

    public ShotData CurrentShotData => currentShotData;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsReleased || HasScored || currentShotData == null)
            return;

        if (Camera.main == null)
            return;

        float playerMovement = Vector3.Distance(Camera.main.transform.position, playerPositionAtRelease);

        if (playerMovement > movementThreshold)
            currentShotData.playerMovedAfterRelease = true;
    }

    public void Initialize(BallSpawnManager manager, HoopManager hoopManagerReference)
    {
        spawnManager = manager;
        hoopManager = hoopManagerReference;
        ResetBallPhysics();
    }

    public void ResetBallPhysics()
    {
        IsReleased = false;
        HasScored = false;
        currentShotData = null;

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

        currentShotData = new ShotData();
        currentShotData.releasePosition = transform.position;
        currentShotData.releaseTime = Time.time;
        currentShotData.releaseForceMagnitude = force.magnitude;
        currentShotData.perfectRelease = CheckPerfectRelease(force);

        if (Camera.main != null)
            playerPositionAtRelease = Camera.main.transform.position;

        if (hoopManager != null && hoopManager.ActiveHoopTransform != null)
        {
            currentShotData.distanceToHoopAtRelease = Vector3.Distance(
                transform.position,
                hoopManager.ActiveHoopTransform.position
            );
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        if (flightStyle)
            force = ApplyFlightStyleRandomness(force);

        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

        if (currentShotData.perfectRelease && perfectReleaseVFXPrefab != null)
            Instantiate(perfectReleaseVFXPrefab, transform.position, Quaternion.identity);

        if (audioSource != null && releaseClip != null)
            audioSource.PlayOneShot(releaseClip);

        missedShotCoroutine = StartCoroutine(MissedShotLifeTimer());
    }

    private bool CheckPerfectRelease(Vector3 force)
    {
        if (Camera.main == null)
            return false;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 forceDirection = force.normalized;

        float alignment = Vector3.Dot(cameraForward, forceDirection);

        return alignment > 0.92f && force.magnitude >= 4f && force.magnitude <= 11f;
    }

    private Vector3 ApplyFlightStyleRandomness(Vector3 originalForce)
    {
        float powerMultiplier = Random.Range(0.55f, 1.65f);
        float sideSwerve = Random.Range(-1.4f, 1.4f);
        float verticalChaos = Random.Range(-0.25f, 0.45f);

        Vector3 randomized = originalForce * powerMultiplier;

        if (Camera.main != null)
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
        if (HasScored)
            return;

        HasScored = true;

        if (currentShotData != null)
        {
            currentShotData.scorePosition = transform.position;
            currentShotData.scoreTime = Time.time;
        }

        if (missedShotCoroutine != null)
        {
            StopCoroutine(missedShotCoroutine);
            missedShotCoroutine = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsReleased || HasScored)
            return;

        ShotSurfaceTag surfaceTag = collision.collider.GetComponentInParent<ShotSurfaceTag>();

        if (surfaceTag != null && currentShotData != null)
            RegisterSurfaceHit(surfaceTag.surfaceType);

        if (audioSource != null && bounceClip != null && collision.relativeVelocity.magnitude > 1f)
            audioSource.PlayOneShot(bounceClip, 0.5f);
    }

    private void RegisterSurfaceHit(ShotSurfaceType surfaceType)
    {
        if (currentShotData == null)
            return;

        switch (surfaceType)
        {
            case ShotSurfaceType.Backboard:
                currentShotData.backboardHitCount++;
                currentShotData.bounceCount++;
                break;

            case ShotSurfaceType.Rim:
                currentShotData.rimHitCount++;
                break;

            case ShotSurfaceType.Floor:
                currentShotData.floorBounceCount++;
                currentShotData.bounceCount++;
                break;

            case ShotSurfaceType.Wall:
                currentShotData.wallBounceCount++;
                currentShotData.bounceCount++;
                break;

            default:
                currentShotData.bounceCount++;
                break;
        }
    }
}