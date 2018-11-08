using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
    public Text roomNameUI;

    void Update () {
        if (ReInput.players.SystemPlayer.GetButtonDown("Restart")) {
            BootGame.roomName = roomNameUI.text;
            SceneManager.LoadScene("GamePlay");
        }
    }
}
