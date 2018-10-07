using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class TestFishController : NetworkBehaviour {
    void Update () {
        if (isLocalPlayer) {
            if (Input.GetButtonDown("Fire1")) {
                CmdTestAction();
            }
        }
    }

    [Command]
    void CmdTestAction () {
        RpcOnTestAction();
    }

    [ClientRpc]
    void RpcOnTestAction () {
        transform.DOPunchScale(0.5f * Vector3.one, 0.5f);
    }
}
