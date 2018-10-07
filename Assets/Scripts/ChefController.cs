using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour {

    public const float knifeXPosMin = -1f;
    public const float knifeXPosMax = 1f;
    public const float knifeYPosMax = 2.3f;
    public const float knifeYPosMin = 1.7f;

    public const float knifeDroppingSpeed = 3f;

    private float gamePlayAreaLeftBoarder;
    private float gamePlayAreaRightBoarder;
    private float gamePlayAreaTopBoarder;
    private float gamePlayAreaBottomBoarder;

    private GameObject knife;

	// Use this for initialization
	void Start () {
        knife = transform.Find("Knife").gameObject;
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
        gamePlayAreaBottomBoarder = 0f;
        gamePlayAreaTopBoarder = Screen.height;
    }

    // Update is called once per frame
    void Update() {
        moveKnifeToMousePosition();
        if (Input.GetMouseButtonUp(0))
        {
            cut();
        }
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
        knife.transform.position = new Vector3(newKnifePosX, newKnifePosY, currKnifePos.z);

    }

    void cut() {
        Animator animator = knife.GetComponentInChildren<Animator>();
        animator.Play("KnifeDown");
    }
}
