using UnityEngine;
using UnityEngine.Networking;

public class ChefController : NetworkBehaviour {

    public const float knifeXPosMin = -1f;
    public const float knifeXPosMax = 1f;
    public const float knifeYPosMax = 3.0f;
    public const float knifeYPosMin = 2.3f;

    public const float fishSelectionActivationBoundary = 50f;

    public const float knifeDroppingSpeed = 3f;

    private float gamePlayAreaLeftBoarder;
    private float gamePlayAreaRightBoarder;
    private float gamePlayAreaTopBoarder;
    private float gamePlayAreaBottomBoarder;
    public Camera viewCamera;
    private Camera fishBasketCamera;

    private bool fishSelectionMode = false;

    private Vector3 knifeRelativePos;

    [SyncVar]
    private Vector2 knifePosition;
    public Animator chefAnimator;

    void Start () {
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
        gamePlayAreaBottomBoarder = 0f;
        gamePlayAreaTopBoarder = Screen.height;

        viewCamera.gameObject.SetActive(isLocalPlayer);
        fishBasketCamera = NetworkGameManager.instance.fishCamera;
        Cursor.visible = false;
    }

    public override void OnStartServer () {
        base.OnStartServer();
    }

    void Update() {
        if (isLocalPlayer) {
            if (Input.GetMouseButtonUp(0))
            {
                CmdCut();
            }
            if (!fishSelectionMode) {
                if (Input.mousePosition.x <= fishSelectionActivationBoundary)
                {
                    switchToFishSelection();
                }
            }
            if (fishSelectionMode)
            {
                if (Input.mousePosition.x >= Screen.width - fishSelectionActivationBoundary)
                {
                    switchToFishCutting();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = fishBasketCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        if (hit.collider.transform.parent.CompareTag("Fish"))
                        {
                            FishControl fish = hit.collider.transform.parent.gameObject.GetComponent<FishControl>();
                            if (!fish.onCuttingBoard && !NetworkGameManager.instance.cuttingBoardTaken)
                            {
                                NetworkGameManager.instance.moveFishToCuttingBoard(fish.gameObject);
                            }
                            else if (fish.onCuttingBoard)
                            {
                                NetworkGameManager.instance.moveFishBackToBasket(fish.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }



    public void switchToFishCutting() {
        viewCamera.gameObject.SetActive(true);
        fishBasketCamera.gameObject.SetActive(false);
        fishSelectionMode = false;
        Cursor.visible = false;
    }

    public void switchToFishSelection() {
        viewCamera.gameObject.SetActive(false);
        fishBasketCamera.gameObject.SetActive(true);
        fishSelectionMode = true;
        Cursor.visible = true;
    }

    [Command]
    void CmdCut () {
        RpcOnCut();
    }

    [ClientRpc]
    void RpcOnCut () {
        chefAnimator.SetTrigger("Cut");
    }
}
