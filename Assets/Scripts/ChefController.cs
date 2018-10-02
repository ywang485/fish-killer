using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour {

    public const float knifeXPosMin = -1f;
    public const float knifeXPosMax = 1f;

    private float gamePlayAreaLeftBoarder;
    private float gamePlayAreaRightBoarder;

    private GameObject knife;

	// Use this for initialization
	void Start () {
        knife = transform.Find("Knife").gameObject;
        gamePlayAreaLeftBoarder = 0f;
        gamePlayAreaRightBoarder = Screen.width;
}

    // Update is called once per frame
    void Update() {
        moveKnifeToMousePosition();
        if(Input.GetMouseButtonUp(0)) {
            cut();
        }
    }

    void moveKnifeToMousePosition() {
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width)
        {
            Vector3 currKnifePos = knife.transform.position;
            float mousePosX = Input.mousePosition.x;
            knife.transform.position = new Vector3(knifeXPosMin + (Input.mousePosition.x / (gamePlayAreaRightBoarder - gamePlayAreaLeftBoarder)) * (knifeXPosMax - knifeXPosMin), currKnifePos.y, currKnifePos.z);
        }
    }

    void cut() {
        Animator animator = knife.GetComponentInChildren<Animator>();
        animator.Play("KnifeDown");
    }
}
