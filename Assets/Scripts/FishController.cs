using UnityEngine;
using UnityEngine.Networking;

public class FishController : NetworkBehaviour
{

    public enum FlopType
    {
        None,
        Flop1,
        Flop2,
        Flop3,
        Flop4
    }

    private GameObject fishBody;

    private float motionStartTime = 0f;
    private float motionDuration = 0f;
    private FishAI fishAI;

    public bool onCuttingBoard = false;
    public Animator fishAnimator;
    public Camera viewCamera;

    void Start() {
        viewCamera.gameObject.SetActive(isLocalPlayer && !isServer);
        // FIXME
        // if (!isLocalPlayer)
        // {
        //     // Plug in random fish AI
        //     int AIIdx = Random.Range(0, 1);
        //     if (AIIdx == 0) {
        //         fishAI = new UniformRandomFishAI();
        //     }
        //     else {
        //         fishAI = new StaticMotionFishAI();
        //     }
        //     FishMotion currMotion = fishAI.nextMotion();
        //     // animator.Play(currMotion.motion); // FIXME
        //     motionStartTime = Time.time;
        //     motionDuration = currMotion.duration;
        // }
    }

    void OnMouseDown(){
        if(!onCuttingBoard && !NetworkGameManager.instance.cuttingBoardTaken) {
            NetworkGameManager.instance.moveFishToCuttingBoard(gameObject);
        } else if (onCuttingBoard) {
            NetworkGameManager.instance.moveFishBackToBasket(gameObject);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
            Debug.Log("???");
                CmdFlop(FlopType.Flop1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CmdFlop(FlopType.Flop2);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CmdFlop(FlopType.Flop3);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CmdFlop(FlopType.Flop4);
            }/* else {
                animator.Play("Still");
            }*/
        }
        else
        {
            //executeAIMotion();
        }
    }

    private void executeAIMotion()
    {
        if (Time.time - motionStartTime >= motionDuration)
        {
            FishMotion currMotion = fishAI.nextMotion();
            //animator.Play(currMotion.motion);// FIXME
            motionStartTime = Time.time;
            motionDuration = currMotion.duration;
        }
    }


    [Command]
    void CmdFlop (FlopType flopType) {
        RpcOnFlop(flopType);
    }

    [ClientRpc]
    void RpcOnFlop (FlopType flopType) {
        switch (flopType) {
        case FlopType.None:
            break;
        case FlopType.Flop1:
            fishAnimator.SetTrigger("Up");
            break;
        case FlopType.Flop2:
            fishAnimator.SetTrigger("Down");
            break;
        case FlopType.Flop3:
            fishAnimator.SetTrigger("Squash");
            break;
        case FlopType.Flop4:
            fishAnimator.SetTrigger("Stretch");
            break;
        }
    }

}
