using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }
}