using UnityEngine;

[CreateAssetMenu(menuName = "AR Showroom/Vehicle Definition")]
public class VehicleDefinition : ScriptableObject
{
    public string vehicleId;
    public string displayName;
    public GameObject prefab;
    public Material[] bodyMaterials;
    public Material[] wheelMaterials;
    public AudioClip engineClip;
    public AudioClip voiceoverClip;
}