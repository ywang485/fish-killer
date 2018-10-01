using UnityEngine.Networking;

public class NetworkGameDiscovery : NetworkDiscovery {
    public event System.Action<string, string> onReceiveBroadcast;
    public override void OnReceivedBroadcast (string fromAddress, string data) {
        if (onReceiveBroadcast != null) onReceiveBroadcast(fromAddress, data);
    }

    public void ClearListeners () {
        onReceiveBroadcast = null;
    }
}