using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ManualBallPlanePlacement : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private BallSpawnManager ballSpawnManager;

    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (GameSessionSettings.Instance.socketMode != BallSocketMode.ManualPlacement)
            return;

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            ballSpawnManager.SpawnBallAtWorldPosition(hits[0].pose.position + Vector3.up * 0.15f);
        }
    }
}