using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowroomUIController : MonoBehaviour
{
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TMP_Text vehicleNameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Slider rotateSpeedSlider;

    private ShowroomRuntime runtime;

    public void Bind(ShowroomRuntime showroomRuntime)
    {
        runtime = showroomRuntime;
        Hide();
    }

    public void Show(string vehicleName)
    {
        rootPanel.SetActive(true);
        vehicleNameText.text = vehicleName;
        statusText.text = "Vehicle ready";
    }

    public void Hide()
    {
        rootPanel.SetActive(false);
    }

    public void OnNextVehicle() => runtime.SwitchVehicle();
    public void OnBodyColor() => runtime.NextBodyColor();
    public void OnWheelColor() => runtime.NextWheelColor();
    public void OnEngineToggle() => runtime.ToggleEngine();
    public void OnRotateToggle() => runtime.ToggleAutoRotate();
    public void OnRotateSpeedChanged(float value) => runtime.SetRotateSpeed(value);
    public void OnReset() => runtime.ResetVehicle();
    public void OnBackToMenu() => runtime.BackToMenu();
}