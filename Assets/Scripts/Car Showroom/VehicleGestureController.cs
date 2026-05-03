using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleGestureController : MonoBehaviour
{
    [SerializeField] private ShowroomRuntime showroomRuntime;
    [SerializeField] private Camera arCamera;
    [SerializeField] private float oneFingerRotateSpeed = 0.2f;
    [SerializeField] private float pinchScaleSpeed = 0.0025f;
    [SerializeField] private float twoFingerRotateMultiplier = 0.5f;

    private float previousTwoFingerAngle;

    private void Update()
    {
        if (!showroomRuntime.HasVehicle()) return;

        if (Input.touchCount == 1)
        {
            HandleOneFinger(Input.GetTouch(0));
        }
        else if (Input.touchCount == 2)
        {
            HandleTwoFinger(Input.GetTouch(0), Input.GetTouch(1));
        }
    }

    private void HandleOneFinger(Touch touch)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (touch.phase == TouchPhase.Moved)
        {
            showroomRuntime.GetActiveVehicle().SetAutoRotate(false);
            showroomRuntime.GetActiveVehicle().RotateManually(-touch.deltaPosition.x * oneFingerRotateSpeed);
        }

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DoorTapTarget target = hit.collider.GetComponent<DoorTapTarget>();
                if (target != null)
                    target.Trigger();
            }
        }
    }

    private void HandleTwoFinger(Touch touch0, Touch touch1)
    {
        if (EventSystem.current != null &&
            (EventSystem.current.IsPointerOverGameObject(touch0.fingerId) ||
             EventSystem.current.IsPointerOverGameObject(touch1.fingerId)))
            return;

        Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
        Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        float pinchDelta = (currentDistance - prevDistance) * pinchScaleSpeed;

        showroomRuntime.GetActiveVehicle().ScaleByDelta(pinchDelta);

        Vector2 currentVector = touch1.position - touch0.position;
        float currentAngle = Mathf.Atan2(currentVector.y, currentVector.x) * Mathf.Rad2Deg;

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            previousTwoFingerAngle = currentAngle;
            return;
        }

        float angleDelta = Mathf.DeltaAngle(previousTwoFingerAngle, currentAngle);
        previousTwoFingerAngle = currentAngle;

        showroomRuntime.GetActiveVehicle().SetAutoRotate(false);
        showroomRuntime.GetActiveVehicle().RotateTwoFinger(-angleDelta * twoFingerRotateMultiplier);
    }
}