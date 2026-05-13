using UnityEngine;

public class ShotData
{
    public Vector3 releasePosition;
    public Vector3 scorePosition;

    public float releaseTime;
    public float scoreTime;

    public float releaseForceMagnitude;
    public float distanceToHoopAtRelease;

    public int bounceCount;
    public int wallBounceCount;
    public int floorBounceCount;
    public int backboardHitCount;
    public int rimHitCount;

    public bool playerMovedAfterRelease;
    public bool perfectRelease;

    public float TimeToScore
    {
        get { return scoreTime - releaseTime; }
    }

    public bool UsedBackboard
    {
        get { return backboardHitCount > 0; }
    }

    public bool BouncedBeforeScoring
    {
        get { return bounceCount > 0; }
    }

    public bool WasLongDistance(float requiredDistance)
    {
        return distanceToHoopAtRelease >= requiredDistance;
    }
}