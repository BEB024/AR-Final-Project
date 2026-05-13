using UnityEngine;

[System.Serializable]
public class TrickShotChallenge
{
    public TrickShotChallengeType challengeType;
    public string challengeName;
    [TextArea] public string description;

    public int basePoints = 2;
    public float requiredDistance = 3f;
    public float timeLimit = 5f;
    public int requiredConsecutiveShots = 2;

    public bool IsCompleted(ShotData shotData, int currentCombo)
    {
        if (shotData == null)
            return false;

        switch (challengeType)
        {
            case TrickShotChallengeType.BankShot:
                return shotData.UsedBackboard;

            case TrickShotChallengeType.BounceShot:
                return shotData.BouncedBeforeScoring;

            case TrickShotChallengeType.LongDistance:
                return shotData.WasLongDistance(requiredDistance);

            case TrickShotChallengeType.QuickShot:
                return shotData.TimeToScore <= timeLimit;

            case TrickShotChallengeType.NoMovementShot:
                return !shotData.playerMovedAfterRelease;

            case TrickShotChallengeType.ConsecutiveShots:
                return currentCombo + 1 >= requiredConsecutiveShots;

            case TrickShotChallengeType.PerfectRelease:
                return shotData.perfectRelease;

            default:
                return false;
        }
    }
}