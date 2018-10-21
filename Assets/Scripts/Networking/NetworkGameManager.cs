using UnityEngine;
using UnityEngine.Networking;

/// This class handles general networking messages, connections
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

    // NOTE this function is only called on the host and never on the client
    // to initialize on the client, put the code in `Start()` or `OnStartClient()` of `FishController`
    public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
        GameObject obj;
        if (conn.address == "localClient" && playerControllerId == 0) {
            obj = instantiateGameForChef();
        } else {
            obj = instantiateGameForFish();
        }
        NetworkServer.AddPlayerForConnection(conn, obj, playerControllerId);
    }

    private GameObject instantiateGameForChef() {
        GameObject obj = Instantiate(chefPrefab);
        return obj;
    }

    private GameObject instantiateGameForFish() {
        GameObject obj = Instantiate(fishPrefab, GameController.instance.fishSpawnPoint.position, Quaternion.identity);
        return obj;
    }
}
