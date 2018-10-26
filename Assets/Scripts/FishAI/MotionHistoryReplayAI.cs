using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionHistoryReplayAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Download Motion History
        MotionHistoryServerCommunication comm = MotionHistoryServerCommunication.instance;
        string historyJson = comm.downloadJSON();
        // Todo: Parse the JSON file
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}