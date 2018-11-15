using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
    public Text roomNameUI;

    public void StartGame () {
        BootGame.roomName = roomNameUI.text;
        SceneManager.LoadScene("GamePlay");
    }
}
