using UnityEngine;

public class HoopManager : MonoBehaviour
{
    [Header("Hoop")]
    [SerializeField] private GameObject hoopPrefab;
    [SerializeField] private BrokenHoopsGameManager gameManager;
    [SerializeField] private GameplayUIManager uiManager;

    private GameObject activeHoop;
    private HoopController activeHoopController;

    public bool HasHoop => activeHoop != null;
    public HoopController ActiveHoopController => activeHoopController;

    public void SpawnHoop(Pose pose)
    {
        if (activeHoop != null)
            Destroy(activeHoop);

        activeHoop = Instantiate(hoopPrefab, pose.position, pose.rotation);
        activeHoopController = activeHoop.GetComponent<HoopController>();

        if (activeHoopController != null)
        {
            activeHoopController.SetBackboardMaterialIndex(
                GameSessionSettings.Instance.selectedBackboardColorIndex
            );
        }

        uiManager.ShowPlacementConfirmPanel();
    }

    public void ConfirmHoopPlacement()
    {
        uiManager.HidePlacementConfirmPanel();
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
