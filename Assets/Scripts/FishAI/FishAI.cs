using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FishAI {

    FishMotion nextMotion();
    void inputData();
}
