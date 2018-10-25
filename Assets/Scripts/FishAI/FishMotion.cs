using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishMotionType
{
    Flop1,
    Flop2,
    Flop3,
    Flop4,
    Still
}

public class FishMotion {

    static public FishMotionType[] predefinedMotions = { FishMotionType.Flop1, FishMotionType.Flop2, FishMotionType.Flop3, FishMotionType.Flop4 };

    public FishMotionType motion;
    public float duration;

    public FishMotion()
    {
        motion = predefinedMotions[0];
        duration = 1f;
    }
}
