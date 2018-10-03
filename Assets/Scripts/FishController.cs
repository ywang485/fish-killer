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
        if (Input.GetMouseButton(0)) {
            animator.Play("FishFlop-1");
        }
        else if (Input.GetMouseButton(1))
        {
            animator.Play("FishFlop-2");
        }
        else {
            animator.Play("Still");
        }

    }
}
