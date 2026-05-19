using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketballController : MonoBehaviour
{
    public bool IsReleased { get; private set; }
    public bool HasScored { get; private set; }
    public bool IsGrabbed { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Flight Style Extra Randomness")]
    [SerializeField] private float flightPowerMin = 0.35f;
    [SerializeField] private float flightPowerMax = 2.25f;
    [SerializeField] private float flightSideSwerve = 4f;
    [SerializeField] private float flightVerticalChaosMin = -1.2f;
    [SerializeField] private float flightVerticalChaosMax = 2.8f;
    [SerializeField] private float flightForwardChaos = 2.5f;
    [SerializeField] private float flightTorqueMin = 2f;
    [SerializeField] private float flightTorqueMax = 12f;

    private BallSpawnManager spawnManager;
    private HoopManager hoopManager;
    private Coroutine missRoutine;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public void Initialize(BallSpawnManager manager, HoopManager hoopManagerReference)
    {
        spawnManager = manager;
        hoopManager = hoopManagerReference;
        ResetForSocket();
    }

    public void ResetForSocket()
    {
        IsReleased = false;
        HasScored = false;
        IsGrabbed = false;

        if (missRoutine != null)
        {
            StopCoroutine(missRoutine);
            missRoutine = null;
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void MoveToSocket(Transform socket)
    {
        if (socket == null)
            return;

        transform.SetPositionAndRotation(socket.position, socket.rotation);
    }

    public void BeginGrab()
    {
        IsGrabbed = true;
        IsReleased = false;
        HasScored = false;

        if (missRoutine != null)
        {
            StopCoroutine(missRoutine);
            missRoutine = null;
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void CancelGrab()
    {
        IsGrabbed = false;
        IsReleased = false;
        HasScored = false;

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void MoveGrabbed(Vector3 worldPosition)
    {
        if (!IsGrabbed)
            return;

        transform.position = worldPosition;
    }

    public void ReleaseBall(Vector3 force, bool flightStyle)
    {
        if (!IsGrabbed || IsReleased)
            return;

        IsGrabbed = false;
        IsReleased = true;
        HasScored = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 finalForce = force;

        if (flightStyle)
            finalForce = ApplyFlightStyleRandomness(force);

        rb.AddForce(finalForce, ForceMode.Impulse);

        float torquePower = flightStyle
            ? Random.Range(flightTorqueMin, flightTorqueMax)
            : 2f;

        rb.AddTorque(Random.insideUnitSphere * torquePower, ForceMode.Impulse);

        missRoutine = StartCoroutine(MissRoutine());

        Debug.Log("BasketballController: Released with force " + finalForce);
    }

    private Vector3 ApplyFlightStyleRandomness(Vector3 originalForce)
    {
        float powerMultiplier = Random.Range(flightPowerMin, flightPowerMax);

        Vector3 randomizedForce = originalForce * powerMultiplier;

        Camera cam = Camera.main;

        if (cam != null)
        {
            randomizedForce += cam.transform.right * Random.Range(-flightSideSwerve, flightSideSwerve);
            randomizedForce += cam.transform.forward * Random.Range(-flightForwardChaos, flightForwardChaos);
        }
        else
        {
            randomizedForce += Vector3.right * Random.Range(-flightSideSwerve, flightSideSwerve);
        }

        randomizedForce += Vector3.up * Random.Range(flightVerticalChaosMin, flightVerticalChaosMax);

        return randomizedForce;
    }

    private IEnumerator MissRoutine()
    {
        yield return new WaitForSeconds(5f);

        if (!HasScored && spawnManager != null)
            spawnManager.RespawnBall(0.1f);
    }

    public void MarkScored()
    {
        if (HasScored)
            return;

        HasScored = true;

        if (missRoutine != null)
        {
            StopCoroutine(missRoutine);
            missRoutine = null;
        }
    }
}

// using System.Collections;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// public class BasketballController : MonoBehaviour
// {
//     public bool IsReleased { get; private set; }
//     public bool HasScored { get; private set; }
//     public bool IsGrabbed { get; private set; }

//     [Header("References")]
//     [SerializeField] private Rigidbody rb;

//     private BallSpawnManager spawnManager;
//     private HoopManager hoopManager;
//     private Coroutine missRoutine;

//     private void Awake()
//     {
//         if (rb == null)
//             rb = GetComponent<Rigidbody>();
//     }

//     public void Initialize(BallSpawnManager manager, HoopManager hoopManagerReference)
//     {
//         spawnManager = manager;
//         hoopManager = hoopManagerReference;
//         ResetForSocket();
//     }

//     public void ResetForSocket()
//     {
//         IsReleased = false;
//         HasScored = false;
//         IsGrabbed = false;

//         if (missRoutine != null)
//         {
//             StopCoroutine(missRoutine);
//             missRoutine = null;
//         }

//         rb.isKinematic = false;
//         rb.useGravity = false;
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         rb.isKinematic = true;
//     }

//     public void MoveToSocket(Transform socket)
//     {
//         if (socket == null)
//             return;

//         transform.SetPositionAndRotation(socket.position, socket.rotation);
//     }

//     public void BeginGrab()
//     {
//         IsGrabbed = true;
//         IsReleased = false;
//         HasScored = false;

//         if (missRoutine != null)
//         {
//             StopCoroutine(missRoutine);
//             missRoutine = null;
//         }

//         rb.isKinematic = false;
//         rb.useGravity = false;
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         rb.isKinematic = true;
//     }

//     public void MoveGrabbed(Vector3 worldPosition)
//     {
//         if (!IsGrabbed)
//             return;

//         transform.position = worldPosition;
//     }

//     public void ReleaseBall(Vector3 force, bool flightStyle)
//     {
//         if (!IsGrabbed && IsReleased)
//             return;

//         IsGrabbed = false;
//         IsReleased = true;
//         HasScored = false;

//         rb.isKinematic = false;
//         rb.useGravity = true;
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;

//         if (flightStyle)
//             force = ApplyFlightStyleRandomness(force);

//         rb.AddForce(force, ForceMode.Impulse);
//         rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);

//         missRoutine = StartCoroutine(MissRoutine());

//         Debug.Log("BasketballController: Released with force " + force);
//     }

//     private Vector3 ApplyFlightStyleRandomness(Vector3 originalForce)
//     {
//         float powerMultiplier = Random.Range(0.55f, 1.65f);
//         float sideSwerve = Random.Range(-1.4f, 1.4f);
//         float verticalChaos = Random.Range(-0.25f, 0.45f);

//         Vector3 randomizedForce = originalForce * powerMultiplier;

//         if (Camera.main != null)
//             randomizedForce += Camera.main.transform.right * sideSwerve;

//         randomizedForce += Vector3.up * verticalChaos;

//         return randomizedForce;
//     }

//     private IEnumerator MissRoutine()
//     {
//         yield return new WaitForSeconds(5f);

//         if (!HasScored && spawnManager != null)
//             spawnManager.RespawnBall(0.1f);
//     }

//     public void MarkScored()
//     {
//         if (HasScored)
//             return;

//         HasScored = true;

//         if (missRoutine != null)
//         {
//             StopCoroutine(missRoutine);
//             missRoutine = null;
//         }
//     }
// }