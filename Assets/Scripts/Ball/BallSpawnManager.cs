// using System.Collections;
// using UnityEngine;

// public class BallSpawnManager : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private Transform cameraTransform;
//     [SerializeField] private Transform socketAnchor;
//     [SerializeField] private HoopManager hoopManager;

//     [Header("Ball Prefabs")]
//     [SerializeField] private GameObject[] basketballPrefabs;

//     [Header("Spawn Settings")]
//     [SerializeField] private float manualSpawnDistance = 1.2f;

//     private BasketballController currentBall;
//     private GameObject currentBallObject;

//     public BasketballController CurrentBall => currentBall;

//     private void Start()
//     {
//         if (cameraTransform == null && Camera.main != null)
//             cameraTransform = Camera.main.transform;
//     }

//     private void Update()
//     {
//         if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
//             KeepBallInSocketBeforeRelease();
//     }

//     public void SpawnBallAtSocket()
//     {
//         ClearExistingBallImmediate();

//         int index = GameSessionSettings.Instance.selectedBallIndex;
//         index = Mathf.Clamp(index, 0, basketballPrefabs.Length - 1);

//         currentBallObject = Instantiate(basketballPrefabs[index], socketAnchor.position, socketAnchor.rotation);
//         currentBall = currentBallObject.GetComponent<BasketballController>();
//         currentBall.Initialize(this, hoopManager);
//     }

//     public void SpawnBallAtWorldPosition(Vector3 position)
//     {
//         ClearExistingBallImmediate();

//         int index = GameSessionSettings.Instance.selectedBallIndex;
//         index = Mathf.Clamp(index, 0, basketballPrefabs.Length - 1);

//         currentBallObject = Instantiate(basketballPrefabs[index], position, Quaternion.identity);
//         currentBall = currentBallObject.GetComponent<BasketballController>();
//         currentBall.Initialize(this, hoopManager);
//     }

//     private void KeepBallInSocketBeforeRelease()
//     {
//         if (currentBall == null)
//             return;

//         if (currentBall.IsReleased)
//             return;

//         if (socketAnchor == null)
//             return;

//         currentBall.transform.SetPositionAndRotation(socketAnchor.position, socketAnchor.rotation);
//     }

//     public void ThrowCurrentBall(Vector3 force)
//     {
//         if (currentBall == null)
//             return;

//         if (currentBall.IsReleased)
//             return;

//         bool flightStyle = GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;
//         currentBall.ReleaseBall(force, flightStyle);
//     }

//     public void HandleBallScored(BasketballController ball)
//     {
//         StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
//     }

//     public void HandleBallOutOfRadius(BasketballController ball)
//     {
//         StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
//     }

//     public void HandleBallMissedInsideRadius(BasketballController ball)
//     {
//         if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
//             StartCoroutine(DestroyAndRespawn(ball.gameObject, 0f));
//     }

//     private IEnumerator DestroyAndRespawn(GameObject ballObject, float delay)
//     {
//         yield return new WaitForSeconds(delay);

//         if (ballObject != null)
//             Destroy(ballObject);

//         currentBall = null;
//         currentBallObject = null;

//         if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
//         {
//             yield return new WaitForSeconds(0.1f);
//             SpawnBallAtSocket();
//         }
//     }

//     public void ClearExistingBallImmediate()
//     {
//         if (currentBallObject != null)
//             Destroy(currentBallObject);

//         currentBall = null;
//         currentBallObject = null;
//     }

//     public void ManualSpawnInFrontOfCamera()
//     {
//         if (cameraTransform == null)
//             return;

//         Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * manualSpawnDistance;
//         SpawnBallAtWorldPosition(spawnPosition);
//     }
// }

using System.Collections;
using UnityEngine;

public class BallSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform socketAnchor;
    [SerializeField] private HoopManager hoopManager;

    [Header("Ball Prefabs")]
    [SerializeField] private GameObject[] basketballPrefabs;

    [Header("Manual Spawn")]
    [SerializeField] private float manualSpawnDistance = 1.2f;

    private GameObject currentBallObject;
    private BasketballController currentBall;

    public BasketballController CurrentBall => currentBall;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (GameSessionSettings.Instance == null)
            return;

        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
            KeepBallInSocketBeforeRelease();
    }

    private void KeepBallInSocketBeforeRelease()
    {
        if (currentBall == null)
            return;

        if (currentBall.IsReleased)
            return;

        if (socketAnchor == null)
            return;

        currentBall.transform.SetPositionAndRotation(socketAnchor.position, socketAnchor.rotation);
    }

    public void SpawnBallAtSocket()
    {
        if (socketAnchor == null)
        {
            Debug.LogError("BallSpawnManager: Socket Anchor is not assigned.");
            return;
        }

        if (basketballPrefabs == null || basketballPrefabs.Length == 0)
        {
            Debug.LogError("BallSpawnManager: No basketball prefabs assigned.");
            return;
        }

        ClearExistingBallImmediate();

        int index = Mathf.Clamp(
            GameSessionSettings.Instance.selectedBallIndex,
            0,
            basketballPrefabs.Length - 1
        );

        currentBallObject = Instantiate(
            basketballPrefabs[index],
            socketAnchor.position,
            socketAnchor.rotation
        );

        currentBall = currentBallObject.GetComponent<BasketballController>();

        if (currentBall == null)
        {
            Debug.LogError("BallSpawnManager: Basketball prefab needs BasketballController on the root.");
            return;
        }

        currentBall.Initialize(this, hoopManager);

        Debug.Log("BallSpawnManager: Ball spawned at socket.");
    }

    public void SpawnBallAtWorldPosition(Vector3 position)
    {
        if (basketballPrefabs == null || basketballPrefabs.Length == 0)
        {
            Debug.LogError("BallSpawnManager: No basketball prefabs assigned.");
            return;
        }

        ClearExistingBallImmediate();

        int index = Mathf.Clamp(
            GameSessionSettings.Instance.selectedBallIndex,
            0,
            basketballPrefabs.Length - 1
        );

        currentBallObject = Instantiate(
            basketballPrefabs[index],
            position,
            Quaternion.identity
        );

        currentBall = currentBallObject.GetComponent<BasketballController>();

        if (currentBall == null)
        {
            Debug.LogError("BallSpawnManager: Basketball prefab needs BasketballController on the root.");
            return;
        }

        currentBall.Initialize(this, hoopManager);

        Debug.Log("BallSpawnManager: Ball spawned at world position.");
    }

    public void ManualSpawnInFrontOfCamera()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("BallSpawnManager: Camera Transform is not assigned.");
            return;
        }

        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * manualSpawnDistance;
        SpawnBallAtWorldPosition(spawnPosition);
    }

    // Old compatibility method used by FlickThrowInput
    public void ThrowCurrentBall(Vector3 force)
    {
        if (currentBall == null)
        {
            Debug.LogWarning("BallSpawnManager: No current ball to throw.");
            return;
        }

        if (currentBall.IsReleased)
            return;

        bool flightStyle = GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;
        currentBall.ReleaseBall(force, flightStyle);
    }

    public void HandleBallScored(BasketballController ball)
    {
        if (ball == null)
            return;

        StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
    }

    public void HandleBallOutOfRadius(BasketballController ball)
    {
        if (ball == null)
            return;

        StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
    }

    public void HandleBallMissedInsideRadius(BasketballController ball)
    {
        if (ball == null)
            return;

        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
            StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.1f));
    }

    public void RespawnBall(float delay)
    {
        StartCoroutine(RespawnRoutine(delay));
    }

    private IEnumerator RespawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBallAtSocket();
    }

    private IEnumerator DestroyAndRespawn(GameObject ballObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ballObject != null)
            Destroy(ballObject);

        currentBallObject = null;
        currentBall = null;

        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBallAtSocket();
        }
    }

    public void ClearExistingBallImmediate()
    {
        if (currentBallObject != null)
            Destroy(currentBallObject);

        currentBallObject = null;
        currentBall = null;
    }

    // Alias for newer scripts
    public void ClearBall()
    {
        ClearExistingBallImmediate();
    }
}