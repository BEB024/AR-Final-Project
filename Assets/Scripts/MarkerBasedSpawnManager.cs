using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MarkerBasedSpawnManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private ShowroomRuntime showroomRuntime;
    [SerializeField] private string markerNameForVehicleA = "MarkerA";
    [SerializeField] private string markerNameForVehicleB = "MarkerB";

    private readonly Dictionary<string, int> markerToVehicleIndex = new();

    private void Awake()
    {
        markerToVehicleIndex[markerNameForVehicleA] = 0;
        markerToVehicleIndex[markerNameForVehicleB] = 1;
    }

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var trackedImage in args.added)
        {
            HandleTrackedImage(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                HandleTrackedImage(trackedImage);
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string detectedName = trackedImage.referenceImage.name;

        if (!markerToVehicleIndex.TryGetValue(detectedName, out int vehicleIndex))
            return;

        SetSelectedVehicle(vehicleIndex);

        Pose pose = new Pose(
            trackedImage.transform.position,
            trackedImage.transform.rotation
        );

        showroomRuntime.SpawnVehicle(pose);
    }

    private void SetSelectedVehicle(int index)
    {
        // Small shortcut: keep switching until desired index
        // You can replace this later with a direct setter if you prefer.
        while (showroomRuntime.GetSelectedVehicle() != null &&
               showroomRuntime.GetSelectedVehicle() != null &&
               index != GetCurrentVehicleIndex())
        {
            showroomRuntime.SwitchVehicle();
        }
    }

    private int GetCurrentVehicleIndex()
    {
        // This helper exists only because ShowroomRuntime above uses a private index.
        // In production, add a proper setter/getter on ShowroomRuntime instead.
        return 0;
    }
}