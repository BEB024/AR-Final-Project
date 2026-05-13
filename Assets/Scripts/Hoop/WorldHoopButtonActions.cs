using UnityEngine;

public class WorldHoopButtonActions : MonoBehaviour
{
    [SerializeField] private HoopController hoopController;

    public void PlayVoiceover()
    {
        if (hoopController != null)
            hoopController.PlayVoiceover();
    }

    public void ResetBall()
    {
        BallSpawnManager manager = FindFirstObjectByType<BallSpawnManager>();

        if (manager != null)
            manager.SpawnBallAtSocket();
    }
}