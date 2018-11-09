using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FishControl))]
public class PlayerFishController : NetworkBehaviour
{
    private FishControl control;
    private GameObject fishBody;

    // For motion sequence recoding
    public FishMotionHistory motionRec;
    private float lastMotionStartTime;
    private FishMotionType currMotion = FishMotionType.Still;

    private AudioSource audioSrc;

    public Camera viewCamera;

    void Awake () {
        control = GetComponent<FishControl>();
        audioSrc = GetComponent<AudioSource>();
    }

    void Start() {
        viewCamera.gameObject.SetActive(isLocalPlayer && !isServer);
        if (isLocalPlayer) {
            motionRec = new FishMotionHistory();
            motionRec.totalDuration = 0;
            motionRec.motions = new List<FishMotion>();
            lastMotionStartTime = Time.time;
        }
        
    }

    public override void OnStartServer () {
        base.OnStartServer();
        GameController.instance.OnPlayerJoin(this);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            FishMotionType motion = FishMotionType.Still; 
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                motion = FishMotionType.Flop1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                motion = FishMotionType.Flop2;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                motion = FishMotionType.Flop3;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                motion = FishMotionType.Flop4;
            }
            if (motion != FishMotionType.Still) {
                CmdFlop(motion);
            }
            // Add new motion record when new motion is performed
            if (currMotion != motion) {
                FishMotion newMotion = new FishMotion();
                newMotion.motion = currMotion;
                newMotion.duration = Time.time - lastMotionStartTime;
                motionRec.motions.Add(newMotion);
                motionRec.totalDuration += newMotion.duration;
                currMotion = motion;
                lastMotionStartTime = Time.time;
            }
        }
    }

    [Command]
    void CmdFlop (FishMotionType flopType) {
        control.RpcOnFlop(flopType);
    }
}
