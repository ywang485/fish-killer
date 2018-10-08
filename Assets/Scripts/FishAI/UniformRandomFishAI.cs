using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformRandomFishAI : FishAI
{
    static public float maxSingleMotionDuration = 2.0f;
    static public float minSingleMotionDuration = 0.1f;

    void FishAI.inputData() {
    }

    FishMotion FishAI.nextMotion() {
        FishMotion newFishMotion = new FishMotion();
        newFishMotion.motion = FishMotion.predefinedMotions[Random.Range(0, FishMotion.predefinedMotions.Length)];
        newFishMotion.duration = Random.Range(minSingleMotionDuration, maxSingleMotionDuration);
        return newFishMotion;
    }
}
