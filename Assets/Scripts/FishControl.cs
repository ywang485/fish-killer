using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System.IO;

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
            UploadMotionHistory(false, GetComponent<PlayerFishController>().motionRec);
        }
        GetComponent<Collider>().enabled = false;
    }

    [Client]
    public void OnMercied () {
        fishAnimator.Play("Mercied");
        GetComponent<Collider>().enabled = false;
        DOVirtual.DelayedCall(10, () => NetworkServer.Destroy(gameObject));
        if (GetComponent<PlayerFishController>() != null)
        {
            UploadMotionHistory(true, GetComponent<PlayerFishController>().motionRec);
        }
    }

    [Client]
    private void UploadMotionHistory(bool recognized, FishMotionHistory history) {
        history.recognized = recognized;
        string desc_json = JsonUtility.ToJson(history);
        string seq_json = "[";
        for (int i = 0; i < history.motions.Count; i ++) {
            seq_json += "{";
            seq_json += ("\"motionType\":\"" + MotionHistoryServerCommunication.motionType2String(history.motions[i].motion) + "\",");
            seq_json += ("\"duration\":" + history.motions[i].duration);
            seq_json += "}";
            if (i < history.motions.Count - 1) {
                seq_json += ",\n";
            }

        }
        seq_json += "]\n";
        File.WriteAllText("tmp_history.json", desc_json + seq_json);
        MotionHistoryServerCommunication comm = MotionHistoryServerCommunication.instance;
        comm.PostJSON("{\"meta\": " + desc_json + ", \"sequence\": " + seq_json + "}");
    }

}
