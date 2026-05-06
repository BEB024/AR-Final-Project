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

    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.Markerless)
            return;

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
        }
    }
}
