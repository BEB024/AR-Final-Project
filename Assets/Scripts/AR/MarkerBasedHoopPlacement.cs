using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerBasedHoopPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private HoopManager hoopManager;

    [Header("Placement Offset")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 0f, 0.4f);
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    private bool hasSpawned;

    private void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.MarkerBased)
            return;

        if (hasSpawned)
            return;

        foreach (ARTrackedImage image in args.added)
            TrySpawnFromImage(image);

        foreach (ARTrackedImage image in args.updated)
            TrySpawnFromImage(image);
    }

    private void TrySpawnFromImage(ARTrackedImage image)
    {
        if (image.trackingState != TrackingState.Tracking)
            return;

        Vector3 worldOffset = image.transform.TransformDirection(positionOffset);

        Pose hoopPose = new Pose(
            image.transform.position + worldOffset,
            image.transform.rotation * Quaternion.Euler(rotationOffsetEuler)
        );

        hoopManager.SpawnHoop(hoopPose);
        hasSpawned = true;
    }

    public void ResetMarkerSpawn()
    {
        hasSpawned = false;
    }
}