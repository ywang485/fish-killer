using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionHistoryServerCommunication : MonoBehaviour
{

    private static string postHistoryURL = "http://39.104.139.188:32184/upload";
    public static string downloadHistoryURL = "http://39.104.139.188:32184/download-random";
    private static MotionHistoryServerCommunication _instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static MotionHistoryServerCommunication instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MotionHistoryServerCommunication>();
            }
            return _instance;
        }
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

    static public string motionType2String(FishMotionType mt)
    {
        if (mt == FishMotionType.Flop1)
        {
            return "Flop1";
        }
        else if (mt == FishMotionType.Flop2)
        {
            return "Flop2";
        }
        else if (mt == FishMotionType.Flop3)
        {
            return "Flop3";
        }
        else if (mt == FishMotionType.Flop4)
        {
            return "Flop4";
        }
        else
        {
            return "Still";
        }

    }

    static public FishMotionType string2motionType(string str)
    {
        if (str == "Flop1")
        {
            return FishMotionType.Flop1;
        }
        else if (str == "Flop2")
        {
            return FishMotionType.Flop2;
        }
        else if (str == "Flop3")
        {
            return FishMotionType.Flop3;
        }
        else if (str == "Flop4")
        {
            return FishMotionType.Flop4;
        }
        else
        {
            return FishMotionType.Still;
        }
    }
}
