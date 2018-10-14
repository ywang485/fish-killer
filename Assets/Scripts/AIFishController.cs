using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(FishControl))]
public class AIFishController : NetworkBehaviour {
    private FishControl control;
    private FishAI fishAI;

    private float motionStartTime = 0f;
    private float motionDuration = 0f;

    void Awake () {
        control = GetComponent<FishControl>();
    }

    void Start () {
        if (isServer) {
            // Plug in random fish AI
            int AIIdx = Random.Range(0, 1);
            if (AIIdx == 0) {
                fishAI = new UniformRandomFishAI();
            }
            else {
                fishAI = new StaticMotionFishAI();
            }
            FishMotion currMotion = fishAI.nextMotion();
            control.RpcOnFlop(currMotion.motion);
            motionStartTime = Time.time;
            motionDuration = currMotion.duration;
        }
    }

    void Update () {
        executeAIMotion();
    }

    private void executeAIMotion()
    {
        if (Time.time - motionStartTime >= motionDuration)
        {
            FishMotion currMotion = fishAI.nextMotion();
            control.RpcOnFlop(currMotion.motion);
            motionStartTime = Time.time;
            motionDuration = currMotion.duration;
        }
    }
}
