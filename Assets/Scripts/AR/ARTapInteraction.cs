using UnityEngine;
using UnityEngine.EventSystems;

public class ARTapInteraction : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HoopPartInteractable hoopPart = hit.collider.GetComponent<HoopPartInteractable>();

            if (hoopPart != null)
                hoopPart.Interact();
        }
    }
}