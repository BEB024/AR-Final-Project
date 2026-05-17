// using UnityEngine;

// public class HoopManager : MonoBehaviour
// {
//     [Header("Hoop")]
//     [SerializeField] private GameObject hoopPrefab;
//     [SerializeField] private BrokenHoopsGameManager gameManager;
//     [SerializeField] private GameplayUIManager uiManager;

//     private GameObject activeHoop;
//     private HoopController activeHoopController;

//     public bool HasHoop => activeHoop != null;
//     public HoopController ActiveHoopController => activeHoopController;
//     public Transform ActiveHoopTransform => activeHoop != null ? activeHoop.transform : null;

//     public void SpawnHoop(Pose pose)
//     {
//         if (activeHoop != null)
//             Destroy(activeHoop);

//         activeHoop = Instantiate(hoopPrefab, pose.position, pose.rotation);
//         activeHoopController = activeHoop.GetComponent<HoopController>();

//         if (activeHoopController != null)
//         {
//             activeHoopController.SetBackboardMaterialIndex(GameSessionSettings.Instance.selectedBackboardColorIndex);
//             activeHoopController.PlaySpawnAnimation();
//         }

//         if (uiManager != null)
//             uiManager.ShowPlacementConfirmPanel();
//     }

//     public void ConfirmHoopPlacement()
//     {
//         if (uiManager != null)
//             uiManager.HidePlacementConfirmPanel();

//         if (gameManager != null)
//             gameManager.StartGameAfterPlacement();
//     }

//     public void ClearHoop()
//     {
//         if (activeHoop != null)
//             Destroy(activeHoop);

//         activeHoop = null;
//         activeHoopController = null;
//     }
// }

using UnityEngine;

public class HoopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject hoopPrefab;
    [SerializeField] private BrokenHoopsGameManager gameManager;
    [SerializeField] private GameplayUIManager uiManager;

    private GameObject activeHoop;
    private HoopController activeHoopController;

    public bool HasHoop => activeHoop != null;
    public Transform ActiveHoopTransform => activeHoop != null ? activeHoop.transform : null;
    public HoopController ActiveHoopController => activeHoopController;

    // New clean method
    public void SpawnHoopAtPose(Pose pose)
    {
        SpawnHoop(pose);
    }

    // Old compatibility method used by MarkerlessHoopPlacement and MarkerBasedHoopPlacement
    public void SpawnHoop(Pose pose)
    {
        if (hoopPrefab == null)
        {
            Debug.LogError("HoopManager: Hoop Prefab is not assigned.");
            return;
        }

        ClearHoop();

        activeHoop = Instantiate(hoopPrefab, pose.position, pose.rotation);
        activeHoopController = activeHoop.GetComponent<HoopController>();

        if (activeHoopController != null)
        {
            activeHoopController.SetBackboardMaterialIndex(
                GameSessionSettings.Instance.selectedBackboardColorIndex
            );

            activeHoopController.PlaySpawnAnimation();
        }

        Debug.Log("HoopManager: Hoop spawned.");

        // If you still use the Confirm button workflow, show confirm panel.
        if (uiManager != null)
        {
            uiManager.ShowPlacementConfirmPanel();
        }
        else
        {
            // If no UI manager is assigned, start immediately.
            StartGameNow();
        }
    }

    // Used by image tracking parenting approach
    public void RegisterExistingHoop(GameObject hoopObject)
    {
        if (hoopObject == null)
        {
            Debug.LogError("HoopManager: Tried to register a null hoop.");
            return;
        }

        ClearHoop();

        activeHoop = hoopObject;
        activeHoopController = activeHoop.GetComponent<HoopController>();

        if (activeHoopController != null)
        {
            activeHoopController.SetBackboardMaterialIndex(
                GameSessionSettings.Instance.selectedBackboardColorIndex
            );

            activeHoopController.PlaySpawnAnimation();
        }

        Debug.Log("HoopManager: Existing hoop registered.");

        if (uiManager != null)
        {
            uiManager.ShowPlacementConfirmPanel();
        }
        else
        {
            StartGameNow();
        }
    }

    // Old UI compatibility method
    public void ConfirmHoopPlacement()
    {
        if (uiManager != null)
            uiManager.HidePlacementConfirmPanel();

        StartGameNow();
    }

    private void StartGameNow()
    {
        if (gameManager == null)
        {
            Debug.LogError("HoopManager: BrokenHoopsGameManager is not assigned.");
            return;
        }

        gameManager.StartGameAfterPlacement();
    }

    public void ClearHoop()
    {
        if (activeHoop != null)
            Destroy(activeHoop);

        activeHoop = null;
        activeHoopController = null;
    }
}