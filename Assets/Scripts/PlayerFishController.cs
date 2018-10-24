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

    public override void OnStartServer () {
        base.OnStartServer();
        GameController.instance.OnPlayerJoin(this);
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
