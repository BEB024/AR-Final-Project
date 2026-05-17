using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BallGrabThrowInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARInputHandler inputHandler;
    [SerializeField] private BallSpawnManager ballSpawnManager;
    [SerializeField] private BrokenHoopsGameManager gameManager;
    [SerializeField] private Camera arCamera;

    [Header("Throw Settings")]
    [SerializeField] private float screenForwardMultiplier = 0.018f;
    [SerializeField] private float screenUpMultiplier = 0.012f;
    [SerializeField] private float worldVelocityMultiplier = 0.65f;
    [SerializeField] private float maxThrowForce = 14f;
    [SerializeField] private float minDragDistance = 20f;

    private BasketballController grabbedBall;

    private Vector2 startScreenPosition;
    private Vector2 currentScreenPosition;

    private Vector3 previousWorldPosition;
    private Vector3 currentWorldPosition;

    private float grabDepth;
    private float startTime;
    private float previousTime;

    private bool isDragging;

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnPressStarted += HandlePressStarted;
            inputHandler.OnPressEnded += HandlePressEnded;
        }
    }

    private void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnPressStarted -= HandlePressStarted;
            inputHandler.OnPressEnded -= HandlePressEnded;
        }
    }

    private void Update()
    {
        if (!isDragging || grabbedBall == null)
            return;

        if (Pointer.current == null)
            return;

        currentScreenPosition = Pointer.current.position.ReadValue();

        Vector3 nextWorldPosition = arCamera.ScreenToWorldPoint(
            new Vector3(currentScreenPosition.x, currentScreenPosition.y, grabDepth)
        );

        previousWorldPosition = currentWorldPosition;
        currentWorldPosition = nextWorldPosition;
        previousTime = Mathf.Max(Time.time, 0.001f);

        grabbedBall.MoveGrabbed(currentWorldPosition);
    }

    private void HandlePressStarted(Vector2 screenPosition)
    {
        if (gameManager == null || !gameManager.GameRunning)
            return;

        if (IsPointerOverUI())
            return;

        if (ballSpawnManager == null || ballSpawnManager.CurrentBall == null)
            return;

        BasketballController currentBall = ballSpawnManager.CurrentBall;

        if (currentBall.IsReleased)
            return;

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;

        BasketballController hitBall = hit.collider.GetComponentInParent<BasketballController>();

        if (hitBall == null || hitBall != currentBall)
            return;

        grabbedBall = hitBall;
        grabbedBall.BeginGrab();

        isDragging = true;

        startScreenPosition = screenPosition;
        currentScreenPosition = screenPosition;

        grabDepth = Vector3.Distance(arCamera.transform.position, grabbedBall.transform.position);
        grabDepth = Mathf.Clamp(grabDepth, 0.45f, 3f);

        currentWorldPosition = grabbedBall.transform.position;
        previousWorldPosition = currentWorldPosition;

        startTime = Time.time;
        previousTime = Time.time;

        Debug.Log("BallGrabThrowInput: Ball grabbed.");
    }

    private void HandlePressEnded(Vector2 screenPosition)
    {
        if (!isDragging || grabbedBall == null)
            return;

        isDragging = false;

        Vector2 screenDelta = screenPosition - startScreenPosition;
        float duration = Mathf.Max(Time.time - startTime, 0.05f);

        if (screenDelta.magnitude < minDragDistance)
        {
            grabbedBall.MoveGrabbed(currentWorldPosition);
            grabbedBall = null;
            Debug.Log("BallGrabThrowInput: Drag too small.");
            return;
        }

        Vector3 worldVelocity = (currentWorldPosition - previousWorldPosition) / Mathf.Max(Time.deltaTime, 0.02f);

        float flickSpeed = screenDelta.magnitude / duration;
        float sensitivity = GameSessionSettings.Instance.throwSensitivity;

        Vector3 forwardForce = arCamera.transform.forward * flickSpeed * screenForwardMultiplier * sensitivity;
        Vector3 upwardForce = Vector3.up * Mathf.Max(screenDelta.y, 0f) * screenUpMultiplier * sensitivity;
        Vector3 worldForce = worldVelocity * worldVelocityMultiplier;

        Vector3 finalForce = forwardForce + upwardForce + worldForce;
        finalForce = Vector3.ClampMagnitude(finalForce, maxThrowForce);

        bool flightStyle = GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;

        grabbedBall.ReleaseBall(finalForce, flightStyle);
        grabbedBall = null;

        Debug.Log("BallGrabThrowInput: Ball thrown with force " + finalForce);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }
}