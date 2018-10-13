using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMotionFishAI : FishAI
{

    private int currMotionIdx = 0;
    private float motionDuration = 0.5f;

    void FishAI.inputData()
    {
    }

    FishMotion FishAI.nextMotion()
    {
        var motions = FishMotion.predefinedMotions;
        FishMotion newFishMotion = new FishMotion();
        newFishMotion.motion = motions[currMotionIdx++];
        currMotionIdx = currMotionIdx % motions.Length;
        newFishMotion.duration = motionDuration;
        return newFishMotion;
    }
}
