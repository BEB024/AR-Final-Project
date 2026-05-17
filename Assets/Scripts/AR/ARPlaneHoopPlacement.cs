using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaneHoopPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARInputHandler inputHandler;
    [SerializeField] private HoopManager hoopManager;
    [SerializeField] private Camera arCamera;

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new();

    private void Awake()
    {
        raycastManager = FindFirstObjectByType<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (inputHandler != null)
            inputHandler.OnPressStarted += TryPlaceHoop;
    }

    private void OnDisable()
    {
        if (inputHandler != null)
            inputHandler.OnPressStarted -= TryPlaceHoop;
    }

    private void TryPlaceHoop(Vector2 screenPosition)
    {
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.Markerless)
            return;

        if (IsPointerOverUI())
            return;

        if (hoopManager == null)
        {
            Debug.LogError("ARPlaneHoopPlacement: HoopManager is not assigned.");
            return;
        }

        if (hoopManager.HasHoop)
            return;

        if (raycastManager == null)
        {
            Debug.LogError("ARPlaneHoopPlacement: No ARRaycastManager found.");
            return;
        }

        bool hitPlane = raycastManager.Raycast(
            screenPosition,
            hits,
            TrackableType.PlaneWithinPolygon
        );

        if (!hitPlane)
        {
            Debug.LogWarning("ARPlaneHoopPlacement: Click did not hit an AR plane.");
            return;
        }

        Pose hitPose = hits[0].pose;

        Quaternion facingCameraRotation = hitPose.rotation;

        if (arCamera != null)
        {
            Vector3 cameraForward = arCamera.transform.forward;
            cameraForward.y = 0f;

            if (cameraForward.sqrMagnitude > 0.001f)
                facingCameraRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        }

        Pose finalPose = new Pose(hitPose.position, facingCameraRotation);

        Debug.Log("ARPlaneHoopPlacement: Plane hit. Spawning hoop.");
        hoopManager.SpawnHoopAtPose(finalPose);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }
}