using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMotion {

    static public string[] predefinedMotions = { "FishFlop-1", "FishFlop-2", "FishFlop-3", "FishFlop-4" };

    public string motion;
    public float duration;

    public FishMotion()
    {
        motion = predefinedMotions[0];
        duration = 1f;
    }
}
