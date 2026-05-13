using UnityEngine;

public class SurfaceHitFeedback : MonoBehaviour
{
    [SerializeField] private HoopController hoopController;
    [SerializeField] private ShotSurfaceType surfaceType;

    private void OnCollisionEnter(Collision collision)
    {
        BasketballController ball = collision.collider.GetComponentInParent<BasketballController>();

        if (ball == null)
            return;

        if (hoopController == null)
            return;

        if (surfaceType == ShotSurfaceType.Backboard)
            hoopController.PlayBackboardHitVFX();

        if (surfaceType == ShotSurfaceType.Rim)
            hoopController.PlayRimHitVFX();
    }
}