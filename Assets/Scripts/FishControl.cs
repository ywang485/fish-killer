using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class FishControl : NetworkBehaviour {

    public Animator fishAnimator;
    public bool onCuttingBoard => GameController.instance.fishOnBoard == this;
    public GameObject brokenModel;
    public GameObject normalModel;
    [HideInInspector] AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

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
    public void RpcMoveTo (Vector3 pos) {
        transform.position = pos;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    [Server]
    public void OnCut () {
        RpcOnCut();
        GameController.instance.OnFishKilled(this);
        DOVirtual.DelayedCall(10, () => NetworkServer.Destroy(gameObject));
    }

    [ClientRpc]
    private void RpcOnCut () {
        fishAnimator.Play("Cut");
        audioSrc.PlayOneShot(Resources.Load(ResourceLib.knifeCutSFX) as AudioClip);
        if (GetComponent<PlayerFishController>() != null) {
            // TODO show bloody fx
        }
        GetComponent<Collider>().enabled = false;
    }

    [Client]
    public void OnMercied () {
        fishAnimator.Play("Mercied");
        GetComponent<Collider>().enabled = false;
        DOVirtual.DelayedCall(10, () => NetworkServer.Destroy(gameObject));
    }
}
