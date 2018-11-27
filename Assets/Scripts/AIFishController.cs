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
            int AIIdx = Random.Range(0, 4);
            if (AIIdx == 0)
            {
                fishAI = new UniformRandomFishAI();
                Debug.Log("Uniform random fish AI set.");
            }
            else if (AIIdx == 1)
            {
                fishAI = new StaticMotionFishAI();
                Debug.Log("Static motion fish AI set.");
            }
            else if (AIIdx == 2)
            {
                fishAI = new DoNothingFishAI();
                Debug.Log("Do nothing fish AI set.");
            } else {
                MotionHistoryReplayAI ai = gameObject.AddComponent<MotionHistoryReplayAI> ();
                fishAI = ai;
                Debug.Log("Motion History Replay fish AI set.");
            }
            FishMotion currMotion = fishAI.nextMotion();
            control.RpcOnFlop(currMotion.motion);
            motionStartTime = Time.time;
            motionDuration = currMotion.duration;
        }
    }

    [ServerCallback]
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
