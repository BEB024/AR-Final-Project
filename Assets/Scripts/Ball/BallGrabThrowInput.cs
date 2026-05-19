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

    [Header("Base Throw Settings")]
    [SerializeField] private float screenForwardMultiplier = 0.018f;
    [SerializeField] private float screenUpMultiplier = 0.012f;
    [SerializeField] private float worldVelocityMultiplier = 0.65f;
    [SerializeField] private float maxThrowForce = 14f;
    [SerializeField] private float minDragDistance = 20f;

    [Header("Flight Style Per-Grab Random Settings")]
    [SerializeField] private float flightForwardMin = 0.45f;
    [SerializeField] private float flightForwardMax = 1.95f;
    [SerializeField] private float flightUpMin = 0.35f;
    [SerializeField] private float flightUpMax = 2.2f;
    [SerializeField] private float flightWorldVelocityMin = 0.25f;
    [SerializeField] private float flightWorldVelocityMax = 1.65f;
    [SerializeField] private float flightSensitivityMin = 0.45f;
    [SerializeField] private float flightSensitivityMax = 2.1f;
    [SerializeField] private float flightMaxForceMin = 7f;
    [SerializeField] private float flightMaxForceMax = 22f;
    [SerializeField] private float flightRandomSideForce = 3.5f;
    [SerializeField] private float flightRandomUpForce = 2.5f;

    private BasketballController grabbedBall;

    private Vector2 startScreenPosition;
    private Vector2 currentScreenPosition;

    private Vector3 previousWorldPosition;
    private Vector3 currentWorldPosition;

    private float grabDepth;
    private float startTime;
    private float previousSampleTime;
    private float currentSampleTime;

    private bool isDragging;

    private float activeForwardMultiplier;
    private float activeUpMultiplier;
    private float activeWorldVelocityMultiplier;
    private float activeMaxThrowForce;
    private float activeSensitivityMultiplier;
    private Vector3 activeFlightExtraForce;

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;

        ResetActiveThrowSettings();
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
        previousSampleTime = currentSampleTime;

        currentWorldPosition = nextWorldPosition;
        currentSampleTime = Time.time;

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

        bool flightStyle = IsFlightStyleMode();

        if (flightStyle)
            RandomizeFlightStyleThrowSettings();
        else
            ResetActiveThrowSettings();

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
        currentSampleTime = Time.time;
        previousSampleTime = currentSampleTime;

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
            grabbedBall.CancelGrab();
            grabbedBall = null;
            Debug.Log("BallGrabThrowInput: Drag too small. Grab cancelled.");
            return;
        }

        float sampleDeltaTime = Mathf.Max(currentSampleTime - previousSampleTime, 0.02f);
        Vector3 worldVelocity = (currentWorldPosition - previousWorldPosition) / sampleDeltaTime;

        float flickSpeed = screenDelta.magnitude / duration;

        float baseSensitivity = 1f;

        if (GameSessionSettings.Instance != null)
            baseSensitivity = GameSessionSettings.Instance.throwSensitivity;

        float finalSensitivity = baseSensitivity * activeSensitivityMultiplier;

        Vector3 forwardForce =
            arCamera.transform.forward *
            flickSpeed *
            activeForwardMultiplier *
            finalSensitivity;

        Vector3 upwardForce =
            Vector3.up *
            Mathf.Max(screenDelta.y, 0f) *
            activeUpMultiplier *
            finalSensitivity;

        Vector3 worldForce =
            worldVelocity *
            activeWorldVelocityMultiplier;

        Vector3 finalForce =
            forwardForce +
            upwardForce +
            worldForce +
            activeFlightExtraForce;

        finalForce = Vector3.ClampMagnitude(finalForce, activeMaxThrowForce);

        bool flightStyle = IsFlightStyleMode();

        grabbedBall.ReleaseBall(finalForce, flightStyle);
        grabbedBall = null;

        Debug.Log("BallGrabThrowInput: Ball thrown with force " + finalForce);
    }

    private void ResetActiveThrowSettings()
    {
        activeForwardMultiplier = screenForwardMultiplier;
        activeUpMultiplier = screenUpMultiplier;
        activeWorldVelocityMultiplier = worldVelocityMultiplier;
        activeMaxThrowForce = maxThrowForce;
        activeSensitivityMultiplier = 1f;
        activeFlightExtraForce = Vector3.zero;
    }

    private void RandomizeFlightStyleThrowSettings()
    {
        activeForwardMultiplier = screenForwardMultiplier * Random.Range(flightForwardMin, flightForwardMax);
        activeUpMultiplier = screenUpMultiplier * Random.Range(flightUpMin, flightUpMax);
        activeWorldVelocityMultiplier = worldVelocityMultiplier * Random.Range(flightWorldVelocityMin, flightWorldVelocityMax);
        activeSensitivityMultiplier = Random.Range(flightSensitivityMin, flightSensitivityMax);
        activeMaxThrowForce = Random.Range(flightMaxForceMin, flightMaxForceMax);

        Vector3 right = arCamera != null ? arCamera.transform.right : Vector3.right;

        activeFlightExtraForce =
            right * Random.Range(-flightRandomSideForce, flightRandomSideForce) +
            Vector3.up * Random.Range(-flightRandomUpForce, flightRandomUpForce);

        Debug.Log(
            "Flight Style randomized: " +
            "Forward=" + activeForwardMultiplier +
            " Up=" + activeUpMultiplier +
            " World=" + activeWorldVelocityMultiplier +
            " Sensitivity=" + activeSensitivityMultiplier +
            " MaxForce=" + activeMaxThrowForce +
            " Extra=" + activeFlightExtraForce
        );
    }

    private bool IsFlightStyleMode()
    {
        return GameSessionSettings.Instance != null &&
               GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }
}

// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.InputSystem;

// public class BallGrabThrowInput : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private ARInputHandler inputHandler;
//     [SerializeField] private BallSpawnManager ballSpawnManager;
//     [SerializeField] private BrokenHoopsGameManager gameManager;
//     [SerializeField] private Camera arCamera;

//     [Header("Throw Settings")]
//     [SerializeField] private float screenForwardMultiplier = 0.018f;
//     [SerializeField] private float screenUpMultiplier = 0.012f;
//     [SerializeField] private float worldVelocityMultiplier = 0.65f;
//     [SerializeField] private float maxThrowForce = 14f;
//     [SerializeField] private float minDragDistance = 20f;

//     private BasketballController grabbedBall;

//     private Vector2 startScreenPosition;
//     private Vector2 currentScreenPosition;

//     private Vector3 previousWorldPosition;
//     private Vector3 currentWorldPosition;

//     private float grabDepth;
//     private float startTime;
//     private float previousTime;

//     private bool isDragging;

//     private void Start()
//     {
//         if (arCamera == null)
//             arCamera = Camera.main;
//     }

//     private void OnEnable()
//     {
//         if (inputHandler != null)
//         {
//             inputHandler.OnPressStarted += HandlePressStarted;
//             inputHandler.OnPressEnded += HandlePressEnded;
//         }
//     }

//     private void OnDisable()
//     {
//         if (inputHandler != null)
//         {
//             inputHandler.OnPressStarted -= HandlePressStarted;
//             inputHandler.OnPressEnded -= HandlePressEnded;
//         }
//     }

//     private void Update()
//     {
//         if (!isDragging || grabbedBall == null)
//             return;

//         if (Pointer.current == null)
//             return;

//         currentScreenPosition = Pointer.current.position.ReadValue();

//         Vector3 nextWorldPosition = arCamera.ScreenToWorldPoint(
//             new Vector3(currentScreenPosition.x, currentScreenPosition.y, grabDepth)
//         );

//         previousWorldPosition = currentWorldPosition;
//         currentWorldPosition = nextWorldPosition;
//         previousTime = Mathf.Max(Time.time, 0.001f);

//         grabbedBall.MoveGrabbed(currentWorldPosition);
//     }

//     private void HandlePressStarted(Vector2 screenPosition)
//     {
//         if (gameManager == null || !gameManager.GameRunning)
//             return;

//         if (IsPointerOverUI())
//             return;

//         if (ballSpawnManager == null || ballSpawnManager.CurrentBall == null)
//             return;

//         BasketballController currentBall = ballSpawnManager.CurrentBall;

//         if (currentBall.IsReleased)
//             return;

//         Ray ray = arCamera.ScreenPointToRay(screenPosition);

//         if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
//             return;

//         BasketballController hitBall = hit.collider.GetComponentInParent<BasketballController>();

//         if (hitBall == null || hitBall != currentBall)
//             return;

//         grabbedBall = hitBall;
//         grabbedBall.BeginGrab();

//         isDragging = true;

//         startScreenPosition = screenPosition;
//         currentScreenPosition = screenPosition;

//         grabDepth = Vector3.Distance(arCamera.transform.position, grabbedBall.transform.position);
//         grabDepth = Mathf.Clamp(grabDepth, 0.45f, 3f);

//         currentWorldPosition = grabbedBall.transform.position;
//         previousWorldPosition = currentWorldPosition;

//         startTime = Time.time;
//         previousTime = Time.time;

//         Debug.Log("BallGrabThrowInput: Ball grabbed.");
//     }

//     private void HandlePressEnded(Vector2 screenPosition)
//     {
//         if (!isDragging || grabbedBall == null)
//             return;

//         isDragging = false;

//         Vector2 screenDelta = screenPosition - startScreenPosition;
//         float duration = Mathf.Max(Time.time - startTime, 0.05f);

//         if (screenDelta.magnitude < minDragDistance)
//         {
//             grabbedBall.MoveGrabbed(currentWorldPosition);
//             grabbedBall = null;
//             Debug.Log("BallGrabThrowInput: Drag too small.");
//             return;
//         }

//         Vector3 worldVelocity = (currentWorldPosition - previousWorldPosition) / Mathf.Max(Time.deltaTime, 0.02f);

//         float flickSpeed = screenDelta.magnitude / duration;
//         float sensitivity = GameSessionSettings.Instance.throwSensitivity;

//         Vector3 forwardForce = arCamera.transform.forward * flickSpeed * screenForwardMultiplier * sensitivity;
//         Vector3 upwardForce = Vector3.up * Mathf.Max(screenDelta.y, 0f) * screenUpMultiplier * sensitivity;
//         Vector3 worldForce = worldVelocity * worldVelocityMultiplier;

//         Vector3 finalForce = forwardForce + upwardForce + worldForce;
//         finalForce = Vector3.ClampMagnitude(finalForce, maxThrowForce);

//         bool flightStyle = GameSessionSettings.Instance.selectedGameMode == GameMode.FlightStyle;

//         grabbedBall.ReleaseBall(finalForce, flightStyle);
//         grabbedBall = null;

//         Debug.Log("BallGrabThrowInput: Ball thrown with force " + finalForce);
//     }

//     private bool IsPointerOverUI()
//     {
//         if (EventSystem.current == null)
//             return false;

//         return EventSystem.current.IsPointerOverGameObject();
//     }
// }