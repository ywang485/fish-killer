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
    public void RpcOnFlop (FishMotionType flopType) {
        //audioSrc.PlayOneShot(Resources.Load(ResourceLib.fishFlopSound) as AudioClip);
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
            }
        });
    }

    [ClientRpc]
    private void RpcOnCut () {
        normalModel.SetActive(false);
        brokenModel.SetActive(true);
        GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<PlayerFishController>() != null) {
            // show bloody fx
            Instantiate(Resources.Load<GameObject>(ResourceLib.bleedingEffect) as GameObject, transform.position, Quaternion.identity);
            try {
                UploadMotionHistory(false, GetComponent<PlayerFishController>().motionRec);
            } catch (System.Exception e) {
                Debug.LogException(e);
            }
            audioSrc.PlayOneShot(Resources.Load(ResourceLib.playerFishCut) as AudioClip);
        } else{
            audioSrc.PlayOneShot(Resources.Load(ResourceLib.knifeCutSFX) as AudioClip);
        }
        GetComponent<Collider>().enabled = false;
        if (isLocalPlayer) {
            GetComponent<PlayerFishController>()?.OnCut();
        }
    }

    [Client]
    public void OnMercied () {
        fishAnimator.Play("Mercied");
        GetComponent<Collider>().enabled = false;
        if (GetComponent<PlayerFishController>() != null) {
            try {
                UploadMotionHistory(true, GetComponent<PlayerFishController>().motionRec);
            } catch (System.Exception e) {
                Debug.LogException(e);
            }
        } else {
            DOVirtual.DelayedCall(10, () => NetworkServer.Destroy(gameObject));
        }
        if (isLocalPlayer) {
            GetComponent<PlayerFishController>()?.OnMercied();
        }
    }

    [Client]
    private void UploadMotionHistory(bool recognized, FishMotionHistory history) {
        history.recognized = recognized;
        if (history.motions.Count < MotionHistoryServerCommunication.minimumHistoryLength) {
            return;
        }
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
