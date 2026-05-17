using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnPressStarted;
    public event Action<Vector2> OnPressEnded;

    [SerializeField] private InputActionReference tapAction;

    private void OnEnable()
    {
        if (tapAction == null || tapAction.action == null)
        {
            Debug.LogError("ARInputHandler: Tap Action is not assigned.");
            return;
        }

        tapAction.action.started += HandleStarted;
        tapAction.action.canceled += HandleCanceled;
        tapAction.action.Enable();
    }

    private void OnDisable()
    {
        if (tapAction == null || tapAction.action == null)
            return;

        tapAction.action.started -= HandleStarted;
        tapAction.action.canceled -= HandleCanceled;
        tapAction.action.Disable();
    }

    private void HandleStarted(InputAction.CallbackContext context)
    {
        if (Pointer.current == null)
            return;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        OnPressStarted?.Invoke(screenPosition);
    }

    private void HandleCanceled(InputAction.CallbackContext context)
    {
        if (Pointer.current == null)
            return;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        OnPressEnded?.Invoke(screenPosition);
    }
}