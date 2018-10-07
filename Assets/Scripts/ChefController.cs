using UnityEngine;
using UnityEngine.Networking;

public class ChefController : NetworkBehaviour {

    public const float knifeXPosMin = -1f;
    public const float knifeXPosMax = 1f;
    public const float knifeYPosMax = 2.3f;
    public const float knifeYPosMin = 1.7f;

    public const float knifeDroppingSpeed = 3f;
    public GameObject knife;

    private float gamePlayAreaLeftBoarder;
    private float gamePlayAreaRightBoarder;
    private float gamePlayAreaTopBoarder;
    private float gamePlayAreaBottomBoarder;

    [SyncVar]
    private Vector2 knifePosition;

    void Start () {
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
        gamePlayAreaBottomBoarder = 0f;
        gamePlayAreaTopBoarder = Screen.height;
    }

    public override void OnStartServer () {
        base.OnStartServer();
        knifePosition = knife.transform.position;
    }

    void Update() {
        if (isServer) { // NOTE Chef is always on the server
            moveKnifeToMousePosition();
            if (Input.GetMouseButtonUp(0))
            {
                CmdCut();
            }
        }
        UpdateKnifePosition();
    }

    void moveKnifeToMousePosition() {
        Vector3 currKnifePos = knife.transform.position;
        float newKnifePosX = knife.transform.position.x;
        float newKnifePosY = knife.transform.position.y;
        if (Input.mousePosition.x >= gamePlayAreaLeftBoarder && Input.mousePosition.x <= gamePlayAreaRightBoarder)
        {
            float mousePosX = Input.mousePosition.x;
            newKnifePosX = knifeXPosMin + (Input.mousePosition.x / (gamePlayAreaRightBoarder - gamePlayAreaLeftBoarder)) * (knifeXPosMax - knifeXPosMin);
        }
        // For mouse wheel controlled knife Y position
        newKnifePosY = currKnifePos.y + knifeDroppingSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

        // For mouse Y axis controlled knife Y  position
        /*if (Input.mousePosition.y >= gamePlayAreaBottomBoarder && Input.mousePosition.y <= gamePlayAreaTopBoarder) {
            float mousePosY = Input.mousePosition.y;
            newKnifePosY = knifeYPosMin + (Input.mousePosition.y / (gamePlayAreaTopBoarder - gamePlayAreaBottomBoarder)) * (knifeYPosMax - knifeYPosMin);

        }*/

        if (newKnifePosY > knifeYPosMax) {
            newKnifePosY = knifeYPosMax;
        } else if (newKnifePosY < knifeYPosMin) {
            newKnifePosY = knifeYPosMin;
        }

        knifePosition = new Vector2(newKnifePosX, newKnifePosY);
    }

    void UpdateKnifePosition () {
        knife.transform.position = new Vector3(knifePosition.x, knifePosition.y, knife.transform.position.z);
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
