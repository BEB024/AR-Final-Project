using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    [SerializeField] private BrokenHoopsGameManager gameManager;
    [SerializeField] private HoopController hoopController;

    private void OnTriggerEnter(Collider other)
    {
        BasketballController ball = other.GetComponentInParent<BasketballController>();

        if (ball == null) return;
        if (ball.HasScored) return;

        ball.MarkScored();

        if (gameManager != null)
            gameManager.AddScore(1);

        if (hoopController != null)
            hoopController.PlayScoreFeedback();

        BallSpawnManager ballSpawnManager = FindObjectOfType<BallSpawnManager>();
        if (ballSpawnManager != null)
            ballSpawnManager.HandleBallScored(ball);
    }
}