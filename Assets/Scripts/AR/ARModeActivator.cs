using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARModeActivator : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private void Start()
    {
        ApplyMode();
    }

    public void ApplyMode()
    {
        bool markerless = GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.Markerless;
        bool markerBased = GameSessionSettings.Instance.selectedSpawnMode == SpawnMode.MarkerBased;

        if (planeManager != null)
            planeManager.enabled = markerless;

        if (raycastManager != null)
            raycastManager.enabled = markerless;

        if (trackedImageManager != null)
            trackedImageManager.enabled = markerBased;
    }
}