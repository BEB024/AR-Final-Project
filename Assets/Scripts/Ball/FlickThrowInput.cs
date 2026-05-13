using UnityEngine;
using UnityEngine.EventSystems;

public class FlickThrowInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallSpawnManager ballSpawnManager;
    [SerializeField] private Camera arCamera;

    [Header("Throw Settings")]
    [SerializeField] private float forwardForceMultiplier = 0.018f;
    [SerializeField] private float upwardForceMultiplier = 0.011f;
    [SerializeField] private float maxForce = 14f;
    [SerializeField] private float minFlickDistance = 35f;

    private Vector2 touchStart;
    private float touchStartTime;
    private bool trackingTouch;

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (touch.phase == TouchPhase.Began)
        {
            touchStart = touch.position;
            touchStartTime = Time.time;
            trackingTouch = true;
        }

        if (touch.phase == TouchPhase.Ended && trackingTouch)
        {
            Vector2 touchEnd = touch.position;
            Vector2 flick = touchEnd - touchStart;

            trackingTouch = false;

            if (flick.magnitude < minFlickDistance)
                return;

            float duration = Mathf.Max(Time.time - touchStartTime, 0.05f);
            float flickSpeed = flick.magnitude / duration;

            float sensitivity = GameSessionSettings.Instance.throwSensitivity;

            Vector3 forwardForce = arCamera.transform.forward * flickSpeed * forwardForceMultiplier * sensitivity;
            Vector3 upwardForce = Vector3.up * Mathf.Max(flick.y, 0f) * upwardForceMultiplier * sensitivity;

            Vector3 finalForce = forwardForce + upwardForce;
            finalForce = Vector3.ClampMagnitude(finalForce, maxForce);

            ballSpawnManager.ThrowCurrentBall(finalForce);
        }
    }
}