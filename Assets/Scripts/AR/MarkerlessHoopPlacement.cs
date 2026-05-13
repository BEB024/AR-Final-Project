using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerlessHoopPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private HoopManager hoopManager;

    [Header("Optional Reticle")]
    [SerializeField] private GameObject placementReticle;

    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.Markerless)
        {
            if (placementReticle != null)
                placementReticle.SetActive(false);

            return;
        }

        UpdateReticle();

        if (hoopManager.HasHoop)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            hoopManager.SpawnHoop(pose);

            if (placementReticle != null)
                placementReticle.SetActive(false);
        }
    }

    private void UpdateReticle()
    {
        if (placementReticle == null)
            return;

        if (hoopManager.HasHoop)
        {
            placementReticle.SetActive(false);
            return;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            placementReticle.SetActive(true);
            placementReticle.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
        }
        else
        {
            placementReticle.SetActive(false);
        }
    }
}