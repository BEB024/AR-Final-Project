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

    private bool spawned;

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
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.MarkerBased)
            return;

        if (spawned)
            return;

        foreach (ARTrackedImage image in args.added)
        {
            TrySpawnFromImage(image);
        }

        foreach (ARTrackedImage image in args.updated)
        {
            TrySpawnFromImage(image);
        }
    }

    private void TrySpawnFromImage(ARTrackedImage image)
    {
        if (image.trackingState != TrackingState.Tracking)
            return;

        Vector3 worldOffset = image.transform.TransformDirection(positionOffset);

        Pose pose = new Pose(
            image.transform.position + worldOffset,
            image.transform.rotation * Quaternion.Euler(rotationOffsetEuler)
        );

        hoopManager.SpawnHoop(pose);
        spawned = true;
    }
}