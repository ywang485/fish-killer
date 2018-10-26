using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionHistoryServerCommunication : MonoBehaviour{

    private string postHistoryURL = "http://127.0.0.1:5000/upload";
    private string downloadHistoryURL = "http://127.0.0.1:5000/download-random";
    private static MotionHistoryServerCommunication _instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static MotionHistoryServerCommunication instance
    {
        get
        {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<MotionHistoryServerCommunication>();
            }
            return _instance;
        }
    }

    public string downloadJSON() {
        WWW www;
        www = new WWW(downloadHistoryURL);
        Debug.Log("Downloading motion history...");
        StartCoroutine(WaitForRequest(www));
        return www.text;
    }

    public WWW PostJSON(string jsonStr)
    {
        WWW www;
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        // convert json string to byte
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);

        //Debug.Log(jsonStr);

        www = new WWW(postHistoryURL, formData, postHeader);
        Debug.Log("Uploading motion history...");
        StartCoroutine(WaitForRequest(www));
        return www;
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    static public string motionType2String(FishMotionType mt) {
        if (mt == FishMotionType.Flop1)
        {
            return "Flop1";
        }
        else if (mt == FishMotionType.Flop2)
        {
            return "Flop2";
        }
        else if (mt == FishMotionType.Flop3) {
            return "Flop3";
        } else if (mt == FishMotionType.Flop4) {
            return "Flop4";
        } else {
            return "Still";
        }

    } 
}
