using UnityEngine;
using UnityEngine.UI;
using OnlineService;

/// boot the game
/// host a game or join a game
public class BootGame : MonoBehaviour {
    public GameObject connectingHint;
    public Text connectingTextUI;
    public Text joinHint;
    private ILobby lobby;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject.DontDestroyOnLoad(gameObject);
        // check if a lobby exists
        // if no, host the game and spawn as the killer
        // if yes, join the game and spawn as a fish
        var service = MultiplayerServiceManager.GetService();
        connectingHint.SetActive(true);
        service.RequestLobbies(5, "", 0).OnReturn(lst => {
            connectingHint.SetActive(true);
            if (lst.Count > 0) {
                connectingTextUI.text = "Joining";
                lst[0].Join("").OnReturn((lobby) => {
                    this.lobby = lobby;
                    ShowJoinHint("Joined As Fish");
                }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
            } else {
                connectingTextUI.text = "Hosting";
                service.CreateLobby("theOne", 20, "", LobbyType.Public, "", 0).OnReturn((lobby) => {
                    this.lobby = lobby;
                    ShowJoinHint("Joined As Chef");
                }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
            }
        }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
    }

    void Update () {
        // NOTE press '-' key to add a fish locally for testing
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Debug.Log("Join a local fish");
            UnityEngine.Networking.ClientScene.AddPlayer(1);
        }
    }

    void OnApplicationQuit () {
        if (lobby != null) {
            lobby.Leave();
            lobby = null;
        }
    }

    void OnDestroy () {
        if (lobby != null) {
            lobby.Leave();
            lobby = null;
        }
    }

    public void ShowJoinHint (string text) {
        joinHint.text = text;
        joinHint.gameObject.SetActive(true);
    }
}
