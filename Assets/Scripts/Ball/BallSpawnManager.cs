using System.Collections;
using UnityEngine;

public class BallSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform socketAnchor;
    [SerializeField] private BrokenHoopsGameManager gameManager;

    [Header("Ball Prefabs")]
    [SerializeField] private GameObject[] basketballPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float manualSpawnDistance = 1.2f;

    private BasketballController currentBall;
    private GameObject currentBallObject;

    public BasketballController CurrentBall => currentBall;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
        {
            KeepBallInSocketBeforeRelease();
        }
    }

    public void SpawnBallAtSocket()
    {
        ClearExistingBallImmediate();

        int index = GameSessionSettings.Instance.selectedBallIndex;
        index = Mathf.Clamp(index, 0, basketballPrefabs.Length - 1);

        currentBallObject = Instantiate(basketballPrefabs[index], socketAnchor.position, socketAnchor.rotation);
        currentBall = currentBallObject.GetComponent<BasketballController>();
        currentBall.Initialize(this);
    }

    public void SpawnBallAtWorldPosition(Vector3 position)
    {
        ClearExistingBallImmediate();

        int index = GameSessionSettings.Instance.selectedBallIndex;
        index = Mathf.Clamp(index, 0, basketballPrefabs.Length - 1);

        currentBallObject = Instantiate(basketballPrefabs[index], position, Quaternion.identity);
        currentBall = currentBallObject.GetComponent<BasketballController>();
        currentBall.Initialize(this);
    }

    private void KeepBallInSocketBeforeRelease()
    {
        if (currentBall == null) return;
        if (currentBall.IsReleased) return;
        if (socketAnchor == null) return;

        currentBall.transform.SetPositionAndRotation(socketAnchor.position, socketAnchor.rotation);
    }

    public void ThrowCurrentBall(Vector3 force)
    {
        if (currentBall == null) return;
        if (currentBall.IsReleased) return;

        bool flightStyle = GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;
        currentBall.ReleaseBall(force, flightStyle);
    }

    public void HandleBallScored(BasketballController ball)
    {
        StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
    }

    public void HandleBallOutOfRadius(BasketballController ball)
    {
        StartCoroutine(DestroyAndRespawn(ball.gameObject, 0.5f));
    }

    public void HandleBallMissedInsideRadius(BasketballController ball)
    {
        if (GameSessionSettings.Instance.socketMode == BallSocketMode.AutoSocket)
            StartCoroutine(DestroyAndRespawn(ball.gameObject, 0f));
    }

    private IEnumerator DestroyAndRespawn(GameObject ballObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ballObject != null)
            Destroy(ballObject);

        currentBall = null;
        currentBallObject = null;

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

        currentBall = null;
        currentBallObject = null;
    }

    public void ManualSpawnInFrontOfCamera()
    {
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * manualSpawnDistance;
        SpawnBallAtWorldPosition(spawnPosition);
    }
}
