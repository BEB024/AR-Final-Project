using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[Serializable]
public class ImagePrefabEntry
{
    public string imageName;
    public GameObject prefab;
}

public class ARImageTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private HoopManager hoopManager;

    [Header("Image Name To Prefab")]
    [SerializeField] private List<ImagePrefabEntry> imagePrefabs = new List<ImagePrefabEntry>();

    [Header("Spawn Offset")]
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 0.2f, 0f);
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 localScale = Vector3.one;

    [Header("Options")]
    [SerializeField] private bool registerAsHoop = true;
    [SerializeField] private bool onlySpawnIfNoHoopExists = true;
    [SerializeField] private bool colorCodeTrackingState = false;

    private readonly Dictionary<string, GameObject> _prefabLookup = new Dictionary<string, GameObject>();
    private readonly Dictionary<TrackableId, GameObject> _spawnedObjects = new Dictionary<TrackableId, GameObject>();

    private void Awake()
    {
        if (imageManager == null)
            imageManager = FindFirstObjectByType<ARTrackedImageManager>();

        if (hoopManager == null)
            hoopManager = FindFirstObjectByType<HoopManager>();

        _prefabLookup.Clear();

        foreach (var entry in imagePrefabs)
        {
            if (entry == null)
                continue;

            if (string.IsNullOrWhiteSpace(entry.imageName))
                continue;

            if (entry.prefab == null)
                continue;

            if (!_prefabLookup.ContainsKey(entry.imageName))
                _prefabLookup[entry.imageName] = entry.prefab;
        }
    }

    private void OnEnable()
    {
        if (imageManager == null)
        {
            Debug.LogError("ARImageTracker: Image Manager is not assigned.");
            return;
        }

        imageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        Debug.Log("ARImageTracker: Listening for tracked images.");
    }

    private void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
            HandleImageAddedOrUpdated(trackedImage);

        foreach (var trackedImage in eventArgs.updated)
            HandleImageAddedOrUpdated(trackedImage);

        foreach (var pair in eventArgs.removed)
            HandleImageRemoved(pair.Key, pair.Value);
    }

    private void HandleImageAddedOrUpdated(ARTrackedImage trackedImage)
    {
        if (trackedImage == null)
            return;

        string imageName = trackedImage.referenceImage.name;

        if (_spawnedObjects.TryGetValue(trackedImage.trackableId, out GameObject existingContent))
        {
            if (existingContent == null)
            {
                _spawnedObjects.Remove(trackedImage.trackableId);
            }
            else
            {
                UpdateTrackingState(existingContent, trackedImage.trackingState);
                return;
            }
        }

        bool canSpawn =
            trackedImage.trackingState == TrackingState.Tracking ||
            trackedImage.trackingState == TrackingState.Limited;

        if (!canSpawn)
            return;

        if (onlySpawnIfNoHoopExists && hoopManager != null && hoopManager.HasHoop)
            return;

        if (!_prefabLookup.TryGetValue(imageName, out GameObject prefab))
        {
            Debug.LogWarning("ARImageTracker: No prefab mapped for image name: " + imageName);
            return;
        }

        GameObject spawnedContent = Instantiate(
            prefab,
            trackedImage.transform.position,
            trackedImage.transform.rotation
        );

        spawnedContent.transform.SetParent(trackedImage.transform, true);
        spawnedContent.transform.localPosition = localPositionOffset;
        spawnedContent.transform.localRotation = Quaternion.Euler(localRotationOffset);
        spawnedContent.transform.localScale = localScale;

        _spawnedObjects[trackedImage.trackableId] = spawnedContent;

        Debug.Log("ARImageTracker: Spawned prefab for image: " + imageName);

        if (registerAsHoop && hoopManager != null)
            hoopManager.RegisterExistingHoop(spawnedContent);
    }

    private void UpdateTrackingState(GameObject content, TrackingState state)
    {
        switch (state)
        {
            case TrackingState.Tracking:
                content.SetActive(true);
                ApplyColor(content, Color.green);
                break;

            case TrackingState.Limited:
                content.SetActive(true);
                ApplyColor(content, Color.yellow);
                break;

            case TrackingState.None:
                content.SetActive(false);
                break;
        }
    }

    private void HandleImageRemoved(TrackableId trackableId, ARTrackedImage trackedImage)
    {
        if (trackedImage != null)
            Debug.Log("ARImageTracker: Image removed: " + trackedImage.referenceImage.name);

        if (_spawnedObjects.TryGetValue(trackableId, out GameObject content))
        {
            if (content != null)
                Destroy(content);

            _spawnedObjects.Remove(trackableId);
        }
    }

    private void ApplyColor(GameObject content, Color color)
    {
        if (!colorCodeTrackingState)
            return;

        Renderer renderer = content.GetComponentInChildren<Renderer>();

        if (renderer != null)
            renderer.material.color = color;
    }

    public void ClearSpawnedContent()
    {
        foreach (GameObject content in _spawnedObjects.Values)
        {
            if (content != null)
                Destroy(content);
        }

        _spawnedObjects.Clear();
    }
}