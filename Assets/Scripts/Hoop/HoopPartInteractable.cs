using UnityEngine;

public class HoopPartInteractable : MonoBehaviour
{
    public enum HoopPart
    {
        Backboard,
        Rim
    }

    [SerializeField] private HoopController hoopController;
    [SerializeField] private HoopPart hoopPart;

    public void Interact()
    {
        if (hoopController == null) return;

        if (hoopPart == HoopPart.Backboard)
            hoopController.OnBackboardTapped();

        if (hoopPart == HoopPart.Rim)
            hoopController.OnRimTapped();
    }
}