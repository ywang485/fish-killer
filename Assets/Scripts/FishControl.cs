using UnityEngine;
using UnityEngine.Networking;

public class FishControl : NetworkBehaviour {

    public Animator fishAnimator;
    public bool onCuttingBoard = false;
    public GameObject brokenModel;
    public GameObject normalModel;

    [ClientRpc]
    public void RpcFishBreak() {
        normalModel.SetActive(false);
        brokenModel.SetActive(true);
    }

    [ClientRpc]
    public void RpcOnFlop (FishMotionType flopType) {
        switch (flopType) {
        case FishMotionType.Flop1:
            fishAnimator.SetTrigger("Up");
            break;
        case FishMotionType.Flop2:
            fishAnimator.SetTrigger("Down");
            break;
        case FishMotionType.Flop3:
            fishAnimator.SetTrigger("Squash");
            break;
        case FishMotionType.Flop4:
            fishAnimator.SetTrigger("Stretch");
            break;
        }
    }

    [ClientRpc]
    public void RpcMoveToBoard (Vector3 pos) {
        transform.position = pos;
    }

    [ClientRpc]
    public void RpcMoveTo (Vector3 pos) {
        transform.position = pos;
    }
}
