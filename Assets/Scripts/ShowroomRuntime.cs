using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowroomRuntime : MonoBehaviour
{
    [SerializeField] private VehicleDefinition[] vehicles;
    [SerializeField] private ShowroomUIController uiController;

    private int selectedVehicleIndex;
    private VehicleController activeVehicle;
    private GameObject activeVehicleObject;
    private Pose initialPose;
    private bool hasInitialPose;
    private bool autoRotateEnabled = true;

    private void Start()
    {
        uiController.Bind(this);
    }

    public VehicleDefinition GetSelectedVehicle()
    {
        return vehicles[selectedVehicleIndex];
    }

    public void SpawnVehicle(Pose pose)
    {
        if (activeVehicleObject != null)
            Destroy(activeVehicleObject);

        VehicleDefinition def = GetSelectedVehicle();
        activeVehicleObject = Instantiate(def.prefab, pose.position, pose.rotation);
        activeVehicle = activeVehicleObject.GetComponent<VehicleController>();
        activeVehicle.Initialize(def);
        activeVehicle.SetAutoRotate(autoRotateEnabled);

        initialPose = pose;
        hasInitialPose = true;

        uiController.Show(def.displayName);
    }

    public bool HasVehicle() => activeVehicle != null;
    public VehicleController GetActiveVehicle() => activeVehicle;

    public void SwitchVehicle()
    {
        selectedVehicleIndex = (selectedVehicleIndex + 1) % vehicles.Length;

        if (hasInitialPose)
            SpawnVehicle(initialPose);
    }

    public void NextBodyColor()
    {
        if (activeVehicle != null) activeVehicle.NextBodyMaterial();
    }

    public void NextWheelColor()
    {
        if (activeVehicle != null) activeVehicle.NextWheelMaterial();
    }

    public void ToggleEngine()
    {
        if (activeVehicle != null) activeVehicle.ToggleEngine();
    }

    public void ToggleAutoRotate()
    {
        autoRotateEnabled = !autoRotateEnabled;
        if (activeVehicle != null) activeVehicle.SetAutoRotate(autoRotateEnabled);
    }

    public void SetRotateSpeed(float value)
    {
        if (activeVehicle != null) activeVehicle.SetRotateSpeed(value);
    }

    public void ResetVehicle()
    {
        if (activeVehicleObject != null && hasInitialPose)
        {
            activeVehicleObject.transform.SetPositionAndRotation(initialPose.position, initialPose.rotation);
            activeVehicleObject.transform.localScale = Vector3.one;
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetSelectedVehicleIndex(int index)
    {
        if (index < 0 || index >= vehicles.Length) return;
        selectedVehicleIndex = index;
    }
}