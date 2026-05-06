using UnityEngine;

public class PlayerRadiusMonitor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private BallSpawnManager ballSpawnManager;

    [Header("Radius")]
    [SerializeField] private float maxDistanceFromPlayer = 8f;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (ballSpawnManager == null) return;
        if (ballSpawnManager.CurrentBall == null) return;
        if (!ballSpawnManager.CurrentBall.IsReleased) return;

        float distance = Vector3.Distance(
            playerCamera.position,
            ballSpawnManager.CurrentBall.transform.position
        );

        if (distance > maxDistanceFromPlayer)
            ballSpawnManager.HandleBallOutOfRadius(ballSpawnManager.CurrentBall);
    }
}
