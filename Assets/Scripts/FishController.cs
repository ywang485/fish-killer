using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour {

    private Animator animator;

    private GameObject fishBody;

    // Use this for initialization
    void Start () {
        fishBody = transform.Find("FishBody").gameObject;
        animator = fishBody.GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow)) {
            animator.Play("FishFlop-1");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.Play("FishFlop-2");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            animator.Play("FishFlop-3");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            animator.Play("FishFlop-4");
        }
        else {
            animator.Play("Still");
        }

    }
}
