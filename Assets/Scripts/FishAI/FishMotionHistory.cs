using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class FishMotionHistory {
    public List<FishMotion> motions;
    public bool recognized;
    public float totalDuration;
}
