using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionHistoryServerCommunication : MonoBehaviour{

    static string hostname = "127.0.0.1";
    static string port = "5000";

    public WWW PostJSON(string jsonStr)
    {
        WWW www;
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        // convert json string to byte
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);

        www = new WWW(hostname, formData, postHeader);
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
}
