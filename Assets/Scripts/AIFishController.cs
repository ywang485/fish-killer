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
        if (!GetComponent<NetworkIdentity>().localPlayerAuthority) {
            // Plug in random fish AI
            int AIIdx = Random.Range(0, 1);
            if (AIIdx == 0) {
                fishAI = new UniformRandomFishAI();
                Debug.Log("Uniform random fish AI set.");
            }
            else {
                fishAI = new StaticMotionFishAI();
                Debug.Log("Static motion fish AI set.");
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
        if (!GetComponent<NetworkIdentity>().localPlayerAuthority)
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
}
