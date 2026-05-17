using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageHoopTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private HoopManager hoopManager;

    [Header("Hoop")]
    [SerializeField] private GameObject hoopPrefab;

    [Header("Local Offset From Marker")]
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 0f, 0.25f);
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 localScale = Vector3.one;

    private GameObject spawnedHoop;
    private ARTrackedImage trackedImageOwner;

    private void OnEnable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (GameSessionSettings.Instance.selectedSpawnMode != SpawnMode.MarkerBased)
            return;

        foreach (ARTrackedImage trackedImage in args.added)
            HandleImageDetected(trackedImage);

        foreach (ARTrackedImage trackedImage in args.updated)
            HandleImageUpdated(trackedImage);
    }

    private void HandleImageDetected(ARTrackedImage trackedImage)
    {
        if (spawnedHoop != null)
            return;

        if (trackedImage.trackingState != TrackingState.Tracking)
            return;

        if (hoopPrefab == null)
        {
            Debug.LogError("ARImageHoopTracker: Hoop prefab is not assigned.");
            return;
        }

        Debug.Log("ARImageHoopTracker: Marker detected: " + trackedImage.referenceImage.name);

        trackedImageOwner = trackedImage;

        spawnedHoop = Instantiate(hoopPrefab);
        spawnedHoop.transform.SetParent(trackedImage.transform, false);
        spawnedHoop.transform.localPosition = localPositionOffset;
        spawnedHoop.transform.localRotation = Quaternion.Euler(localRotationOffset);
        spawnedHoop.transform.localScale = localScale;

        if (hoopManager != null)
            hoopManager.RegisterExistingHoop(spawnedHoop);
    }

    private void HandleImageUpdated(ARTrackedImage trackedImage)
    {
        if (spawnedHoop == null)
            return;

        if (trackedImage != trackedImageOwner)
            return;

        bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
        spawnedHoop.SetActive(isTracking);
    }
}