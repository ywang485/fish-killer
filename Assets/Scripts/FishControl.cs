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
    public void RpcMoveTo (Vector3 pos, bool fixedRotation) {
        transform.position = pos;
        if (fixedRotation) transform.rotation = Quaternion.identity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    [Server]
    public void OnCut () {
        RpcOnCut();
        GameController.instance.OnFishKilled(this);
        DOVirtual.DelayedCall(5, () => {
            var playerController = GetComponent<PlayerFishController>();
            if (playerController == null) {
                NetworkServer.Destroy(gameObject);
            } else {
                this.RpcMoveTo(GameController.instance.playerSpawnPoint.position, false);
                normalModel.SetActive(true);
                brokenModel.SetActive(false);
                GameController.instance.ResetPlayer(playerController);
            }
        });
    }

    [ClientRpc]
    private void RpcOnCut () {
        normalModel.SetActive(false);
        brokenModel.SetActive(true);
        GetComponent<Rigidbody>().isKinematic = true;
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
