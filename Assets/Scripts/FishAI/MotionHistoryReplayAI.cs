using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionHistoryReplayAI : MonoBehaviour, FishAI {

    private List<FishMotion> motionSeq;
    private int currMotionIdx;
    private bool hasData;

    void Start () {
        hasData = false;
        StartCoroutine(DownloadHistory());
    }

    public IEnumerator DownloadHistory()
    {
        WWW www;
        www = new WWW(MotionHistoryServerCommunication.downloadHistoryURL);
        Debug.Log("Downloading motion history...");
        yield return www;
        Debug.Log("Motion History Retrieved: " + www.text);
        setData(www.text);
    }

    public void setData(string historyStr) {
        motionSeq = new List<FishMotion>();
        string[] motionStrs = historyStr.Split('\n');
        foreach (string motionStr in motionStrs)
        {
            if (!motionStr.Contains("&"))
            {
                continue;
            }
            string[] pair = motionStr.Split('&');
            FishMotion m = new FishMotion();
            m.motion = MotionHistoryServerCommunication.string2motionType(pair[0]);
            m.duration = float.Parse(pair[1]);
            motionSeq.Add(m);
        }
        hasData = true;
        currMotionIdx = 0;
    }

    void FishAI.inputData()
    {
    }

    FishMotion FishAI.nextMotion()
    {
        if (hasData)
        {
            FishMotion toReturn = motionSeq[currMotionIdx];
            currMotionIdx = (currMotionIdx + 1) % motionSeq.Count;
            return toReturn;
        }
        else {
            FishMotion m = new FishMotion();
            m.motion = FishMotionType.Still;
            m.duration = 1f;
            return m;
        }

    }

}