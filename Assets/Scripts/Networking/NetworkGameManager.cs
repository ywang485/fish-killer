using UnityEngine;
using UnityEngine.Networking;

public class NetworkGameManager : NetworkManager {
    public static NetworkGameManager instance {
        get {
            return singleton as NetworkGameManager;
        }
    }

    public GameObject networkDiscoveryPrefab;
    public NetworkGameDiscovery discovery { get; private set; }

    public delegate void ConnectionStatusHandler (bool connected);
    public event ConnectionStatusHandler onClientConnectionStatusChanged;
    public bool dontAcceptNewClients { get; set; }

    public GameObject chefPrefab;
    public GameObject fishPrefab;

    public void StopDiscovery () {
        if (discovery != null && discovery.running) {
            discovery.StopBroadcast();
            discovery.ClearListeners();
        }
    }

    public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
        GameObject obj;
        if (conn.address == "localClient") {
            obj = Instantiate(chefPrefab);
        } else {
            obj = Instantiate(fishPrefab);
        }
        NetworkServer.AddPlayerForConnection(conn, obj, playerControllerId);
    }
}