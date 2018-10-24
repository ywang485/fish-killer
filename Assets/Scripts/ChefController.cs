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

    public Camera viewCamera;
    private Camera fishBasketCamera;

    [HideInInspector] public AudioSource audioSrc;

    private bool fishSelectionMode = false;

    public Animator chefAnimator;
    [SyncVar(hook="OnKnifePosYChanged")]
    private float knifePosY;

    void Start () {
        viewCamera.gameObject.SetActive(isLocalPlayer);
        fishBasketCamera = GameController.instance.basketCamera;
        Cursor.visible = false;

        audioSrc = GetComponent<AudioSource>();
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
                if (ReInput.players.SystemPlayer.GetButtonDown("Cut")) {
                    CmdCut();
                }
                if (ReInput.players.SystemPlayer.GetButtonDown("Mercy")) {
                    CmdMercy();
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
        audioSrc.PlayOneShot(Resources.Load(ResourceLib.knifeSwooshSFX) as AudioClip);
    }

    [Command]
    void CmdMercy () {
        var fish = GameController.instance.fishOnBoard;
        if (fish) {
            RpcOnMercy(fish.gameObject);
            GameController.instance.OnMercyFish(fish);
        }
    }

    [ServerCallback]
    public void OnKnifeDown () {
        GameController.instance.fishOnBoard?.OnCut();
    }

    [ClientRpc]
    void RpcOnCut () {
        chefAnimator.SetTrigger("Cut");
    }

    [ClientRpc]
    void RpcOnMercy (GameObject fish) {
        fish.GetComponent<FishControl>().OnMercied();
    }

    void OnKnifePosYChanged (float posY) {
        chefAnimator.SetFloat("Idle Blend", posY);
    }
}
