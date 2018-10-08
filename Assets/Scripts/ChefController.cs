using UnityEngine;
using UnityEngine.Networking;

public class ChefController : NetworkBehaviour {

    public const float knifeXPosMin = -1f;
    public const float knifeXPosMax = 1f;
    public const float knifeYPosMax = 3.0f;
    public const float knifeYPosMin = 2.3f;

    public const float knifeDroppingSpeed = 3f;
    public GameObject knife;

    private float gamePlayAreaLeftBoarder;
    private float gamePlayAreaRightBoarder;
    private float gamePlayAreaTopBoarder;
    private float gamePlayAreaBottomBoarder;
    public Camera viewCamera;

    private Vector3 knifeRelativePos;

    [SyncVar]
    private Vector2 knifePosition;

    void Start () {
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
        gamePlayAreaBottomBoarder = 0f;
        gamePlayAreaTopBoarder = Screen.height;

        viewCamera.gameObject.SetActive(isLocalPlayer);
        Cursor.visible = false;
    }

    public override void OnStartServer () {
        base.OnStartServer();
    }

    void Update() {
        if (isServer) { // NOTE Chef is always on the server
            if (Input.GetMouseButtonUp(0))
            {
                CmdCut();
            }
        }
        UpdateKnifeTransform();
    }

    private void UpdateKnifeTransform () {
        var chefRotation = Quaternion.Euler(transform.eulerAngles.x, viewCamera.transform.eulerAngles.y, viewCamera.transform.eulerAngles.z);
        transform.rotation = chefRotation;
    }

    [Command]
    void CmdCut () {
        RpcOnCut();
    }

    [ClientRpc]
    void RpcOnCut () {
        Animator animator = knife.GetComponentInChildren<Animator>();
        animator.Play("KnifeDown");
    }
}
