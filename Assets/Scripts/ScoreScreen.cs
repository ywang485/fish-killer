using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour {
    public Text detailsUI;

    public void Show (GameController game, bool success) {
        detailsUI.text =
            $@"{(success ? "Your job is done." : $"You killed {game.fishToKill} fish before an accident happened.")}
Spared Fish: {game.allFishMercied}
Spared Players: {game.playersMercied}
Correct Rate: {(game.allFishMercied == 0 ? 0f : (float)game.playersMercied / game.allFishMercied) * 100}%";
    }
}
