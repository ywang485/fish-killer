using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(FishControl))]
public class PlayerFishController : NetworkBehaviour
{
    private FishControl control;
    private GameObject fishBody;

    public Camera viewCamera;

    void Awake () {
        control = GetComponent<FishControl>();
    }

    void Start() {
        viewCamera.gameObject.SetActive(isLocalPlayer && !isServer);
    }

    void OnMouseDown(){
        // FIXME this part of code should be in chef
        // TODO network sync
        if(!control.onCuttingBoard && !NetworkGameManager.instance.cuttingBoardTaken) {
            NetworkGameManager.instance.moveFishToCuttingBoard(gameObject);
        } else if (control.onCuttingBoard) {
            NetworkGameManager.instance.moveFishBackToBasket(gameObject);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CmdFlop(FishMotionType.Flop1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CmdFlop(FishMotionType.Flop2);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CmdFlop(FishMotionType.Flop3);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CmdFlop(FishMotionType.Flop4);
            }/* else {
                animator.Play("Still");
            }*/
        }
    }

    [Command]
    void CmdFlop (FishMotionType flopType) {
        control.RpcOnFlop(flopType);
    }
}
