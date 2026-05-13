using System.Collections.Generic;
using UnityEngine;

public class TrickShotChallengeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameplayUIManager uiManager;

    [Header("Challenges")]
    [SerializeField] private List<TrickShotChallenge> availableChallenges = new List<TrickShotChallenge>();

    [Header("Runtime")]
    [SerializeField] private TrickShotChallenge activeChallenge;
    [SerializeField] private int combo;
    [SerializeField] private int completedChallenges;

    public TrickShotChallenge ActiveChallenge => activeChallenge;
    public int Combo => combo;
    public int CompletedChallenges => completedChallenges;

    private void Start()
    {
        BuildDefaultChallenges();

        if (GameSessionSettings.Instance.selectedGameMode == GameMode.TrickShot)
            PickNewChallenge();
    }

    private void BuildDefaultChallenges()
    {
        if (availableChallenges.Count > 0)
            return;

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.BankShot,
            challengeName = "Bank Shot",
            description = "Score after hitting the backboard.",
            basePoints = 3
        });

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.BounceShot,
            challengeName = "Bounce Shot",
            description = "Score after the ball bounces at least once.",
            basePoints = 3
        });

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.LongDistance,
            challengeName = "Long Distance",
            description = "Score from at least 3 meters away.",
            basePoints = 4,
            requiredDistance = 3f
        });

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.QuickShot,
            challengeName = "Fast Shot",
            description = "Score within 5 seconds.",
            basePoints = 4,
            timeLimit = 5f
        });

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.PerfectRelease,
            challengeName = "Perfect Release",
            description = "Score with a clean forward release.",
            basePoints = 5
        });

        availableChallenges.Add(new TrickShotChallenge
        {
            challengeType = TrickShotChallengeType.ConsecutiveShots,
            challengeName = "Hot Streak",
            description = "Make 2 shots in a row.",
            basePoints = 5,
            requiredConsecutiveShots = 2
        });
    }

    public void PickNewChallenge()
    {
        if (availableChallenges.Count == 0)
            return;

        int index = Random.Range(0, availableChallenges.Count);
        activeChallenge = availableChallenges[index];

        if (uiManager != null)
            uiManager.ShowChallenge(activeChallenge.challengeName, activeChallenge.description, combo);
    }

    public int EvaluateShot(ShotData shotData)
    {
        if (GameSessionSettings.Instance.selectedGameMode != GameMode.TrickShot)
            return 1;

        if (activeChallenge == null)
            PickNewChallenge();

        bool completed = activeChallenge.IsCompleted(shotData, combo);

        if (completed)
        {
            combo++;
            completedChallenges++;

            int points = activeChallenge.basePoints * Mathf.Max(1, combo);

            if (uiManager != null)
                uiManager.ShowChallengeResult("Challenge Complete! +" + points + " points", combo);

            PickNewChallenge();
            return points;
        }
        else
        {
            combo = 0;

            if (uiManager != null)
                uiManager.ShowChallengeResult("Shot made, but challenge failed.", combo);

            PickNewChallenge();
            return 1;
        }
    }

    public void ResetTrickShotProgress()
    {
        combo = 0;
        completedChallenges = 0;
        PickNewChallenge();
    }
}