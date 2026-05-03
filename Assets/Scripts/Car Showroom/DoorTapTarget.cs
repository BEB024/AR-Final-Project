using UnityEngine;

public class DoorTapTarget : MonoBehaviour
{
    public enum DoorSide { Left, Right }

    [SerializeField] private VehicleController vehicleController;
    [SerializeField] private DoorSide doorSide;

    public void Trigger()
    {
        if (doorSide == DoorSide.Left)
            vehicleController.ToggleLeftDoor();
        else
            vehicleController.ToggleRightDoor();
    }
}