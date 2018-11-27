using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingFishAI : FishAI
{

    void FishAI.inputData()
    {
    }

    FishMotion FishAI.nextMotion()
    {
        FishMotion newFishMotion = new FishMotion();
        newFishMotion.motion = FishMotionType.Still;
        newFishMotion.duration = 1.0f;
        return newFishMotion;
    }
}
