using UnityEngine;
using UnityEngine.Networking;
using Rewired;

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

    public Animator chefAnimator;
    [SyncVar(hook="OnKnifePosYChanged")]
    private float knifePosY;

    void Start () {
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
        gamePlayAreaBottomBoarder = 0f;
        gamePlayAreaTopBoarder = Screen.height;

        viewCamera.gameObject.SetActive(isLocalPlayer);
        fishBasketCamera = GameController.instance.basketCamera;
        Cursor.visible = false;
    }

    public override void OnStartServer () {
        base.OnStartServer();
    }

    void Update() {
        if (isLocalPlayer) {
            var knifeVerticalInput = ReInput.players.SystemPlayer.GetAxis("Knife Vertical");
            if (Mathf.Abs(knifeVerticalInput) > Mathf.Epsilon) {
                knifePosY = Mathf.Clamp01(knifePosY + 0.02f * knifeVerticalInput);
            }
            if (!fishSelectionMode) {
                if (Input.mousePosition.x <= fishSelectionActivationBoundary)
                {
                    switchToFishSelection();
                }
            }
            // for testing
            if (Input.GetKeyDown(KeyCode.N)) {
                GameController.instance.NextFish();
            }
            // end testing
            if (fishSelectionMode)
            {
                if (Input.mousePosition.x >= Screen.width - fishSelectionActivationBoundary)
                {
                    switchToFishCutting();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Fish Selection Mode");
                    RaycastHit hit;
                    Ray ray = fishBasketCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        Debug.Log("Clicked something!");
                        if (hit.collider.transform.CompareTag("Fish"))
                        {
                            FishControl fish = hit.collider.gameObject.GetComponent<FishControl>();
                            if (!fish.onCuttingBoard && !GameController.instance.cuttingBoardTaken)
                            {
                                GameController.instance.moveFishToCuttingBoard(fish);
                                switchToFishCutting();
                            }
                            else if (fish.onCuttingBoard)
                            {
                                GameController.instance.moveFishBackToBasket(fish);
                                switchToFishSelection();
                            }
                        }
                    }
                }
            } else {
                if (Input.GetMouseButtonDown(0)) {
                    CmdCut();
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

    [ServerCallback]
    public void OnKnifeDown () {
        GameController.instance.fishOnBoard?.OnCut();
    }

    [ClientRpc]
    void RpcOnCut () {
        chefAnimator.SetTrigger("Cut");
    }

    void OnKnifePosYChanged (float posY) {
        chefAnimator.SetFloat("Idle Blend", posY);
    }
}
