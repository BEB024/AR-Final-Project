using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTracker : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private XRReferenceImageLibrary referenceImageLibrary;
    [SerializeField] private GameObject contentPrefab;

    [Header("Settings")]
    [SerializeField] private int maxMovingImages = 2;

    private void Awake()
    {
        if (imageManager == null)
            imageManager = FindFirstObjectByType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (!ValidateReferences())
            return;

        ForceAssignImageLibrary();

        imageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);

        Debug.Log("ARImageTracker: Listening for tracked images.");
    }

    private void Start()
    {
        // Run again after XR Simulation initializes, because your project is clearing the library on Play.
        if (ValidateReferences())
            ForceAssignImageLibrary();
    }

    private void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private bool ValidateReferences()
    {
        if (imageManager == null)
        {
            Debug.LogError("ARImageTracker: Image Manager is missing. Drag XR Origin (AR) into the field.");
            return false;
        }

        if (referenceImageLibrary == null)
        {
            Debug.LogError("ARImageTracker: Reference Image Library is missing.");
            return false;
        }

        if (contentPrefab == null)
        {
            Debug.LogError("ARImageTracker: Content Prefab is missing. Assign TestCube first.");
            return false;
        }

        return true;
    }

    private void ForceAssignImageLibrary()
    {
        bool wasEnabled = imageManager.enabled;

        imageManager.enabled = false;
        imageManager.referenceLibrary = referenceImageLibrary;
        imageManager.requestedMaxNumberOfMovingImages = maxMovingImages;
        imageManager.enabled = wasEnabled;

        Debug.Log(
            "ARImageTracker: Forced library assignment: " +
            referenceImageLibrary.name +
            " | Images: " +
            referenceImageLibrary.count
        );
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Added Trackables
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log("ARImageTracker: Image added: " + trackedImage.referenceImage.name);

            GameObject spawnedContent = Instantiate(
                contentPrefab,
                trackedImage.transform.position,
                trackedImage.transform.rotation
            );

            spawnedContent.transform.SetParent(trackedImage.transform);
        }

        // Updated Trackables
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.transform.childCount > 0)
            {
                GameObject content = trackedImage.transform.GetChild(0).gameObject;

                bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

                content.SetActive(isTracking);
            }
        }

        // Removed Trackables
        foreach (var pair in eventArgs.removed)
        {
            ARTrackedImage removedImage = pair.Value;
            Debug.Log("ARImageTracker: Image removed: " + removedImage.referenceImage.name);
        }
    }
}