// using UnityEngine;

// public class ScoreTrigger : MonoBehaviour
// {
//     [SerializeField] private BrokenHoopsGameManager gameManager;
//     [SerializeField] private HoopController hoopController;
//     [SerializeField] private TrickShotChallengeManager trickShotChallengeManager;

//     private void Start()
//     {
//         if (gameManager == null)
//             gameManager = FindFirstObjectByType<BrokenHoopsGameManager>();

//         if (trickShotChallengeManager == null)
//             trickShotChallengeManager = FindFirstObjectByType<TrickShotChallengeManager>();

//         if (hoopController == null)
//             hoopController = GetComponentInParent<HoopController>();
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         BasketballController ball = other.GetComponentInParent<BasketballController>();

//         if (ball == null)
//             return;

//         if (ball.HasScored)
//             return;

//         ball.MarkScored();

//         int pointsToAdd = 1;

//         if (GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot && trickShotChallengeManager != null)
//             pointsToAdd = trickShotChallengeManager.EvaluateShot(ball.CurrentShotData);

//         if (gameManager != null)
//             gameManager.AddScore(pointsToAdd);

//         if (hoopController != null)
//             hoopController.PlayScoreFeedback();

//         BallSpawnManager ballSpawnManager = FindFirstObjectByType<BallSpawnManager>();

//         if (ballSpawnManager != null)
//             ballSpawnManager.HandleBallScored(ball);
//     }
// }

using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BasketballController ball = other.GetComponentInParent<BasketballController>();

        if (ball == null)
            return;

        if (ball.HasScored)
            return;

        ball.MarkScored();

        BrokenHoopsGameManager gameManager = FindFirstObjectByType<BrokenHoopsGameManager>();
        BallSpawnManager ballSpawnManager = FindFirstObjectByType<BallSpawnManager>();

        if (gameManager != null)
            gameManager.AddScore(1);

        if (ballSpawnManager != null)
            ballSpawnManager.RespawnBall(0.5f);

        Debug.Log("ScoreTrigger: Score detected.");
    }
}