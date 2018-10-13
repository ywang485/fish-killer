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
    private Animator animator;

    private GameObject fishBody;

    private float motionStartTime = 0f;
    private float motionDuration = 0f;
    private FishAI fishAI;

    public bool onCuttingBoard = false;

    void Start()
    {
        fishBody = transform.Find("FishBody").gameObject;
        animator = fishBody.GetComponentInChildren<Animator>();
        if (!isLocalPlayer)
        {
            // Plug in random fish AI
            int AIIdx = Random.Range(0, 1);
            if (AIIdx == 0) {
                fishAI = new UniformRandomFishAI();
            }
            else {
                fishAI = new StaticMotionFishAI();
            }
            FishMotion currMotion = fishAI.nextMotion();
            animator.Play(currMotion.motion);
            motionStartTime = Time.time;
            motionDuration = currMotion.duration;
        }
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
            executeAIMotion();
        }
    }

    private void executeAIMotion()
    {
        if (Time.time - motionStartTime >= motionDuration)
        {
            FishMotion currMotion = fishAI.nextMotion();
            animator.Play(currMotion.motion);
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
            animator.Play("Still");
            break;
        case FlopType.Flop1:
            animator.Play("FishFlop-1");
            break;
        case FlopType.Flop2:
            animator.Play("FishFlop-2");
            break;
        case FlopType.Flop3:
            animator.Play("FishFlop-3");
            break;
        case FlopType.Flop4:
            animator.Play("FishFlop-4");
            break;
        }
    }

}
