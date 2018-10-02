using UnityEngine;
using UnityEngine.UI;
using OnlineService;

/// boot the game
/// host a game or join a game
public class BootGame : MonoBehaviour {
    public GameObject connectingHint;
    public Text connectingTextUI;
    private ILobby lobby;
    void Start () {
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
                }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
            } else {
                connectingTextUI.text = "Hosting";
                service.CreateLobby("theOne", 20, "", LobbyType.Public, "", 0).OnReturn((lobby) => {
                    this.lobby = lobby;
                }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
            }
        }).OnError(Debug.LogError).OnFinally(() => connectingHint.SetActive(false));
    }
}
