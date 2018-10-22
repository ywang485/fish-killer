using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnTriggerEnter(Collider c ) {
        if (c.transform.parent.CompareTag("Fish")) {
            Debug.Log("Fish Cut!");
            FishControl fc = c.gameObject.GetComponent<FishControl>();
            fc.RpcFishBreak();
        }
    }
}
