using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

/// This class deals with game-specific logict
public class GameController : NetworkBehaviour {
    static public GameController instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<GameController>();
            }
            return _instance;
        }
    }
    static private GameController _instance;

    public GameObject aiFishPrefab;
    public Transform fishSpawnPoint;
    public int generatedAIFishCount;
    public int fishToCut { get; private set; } // reserved if fishToCut might not be designed as the count of AI fishes
    public List<FishControl> fishList { get; } = new List<FishControl>();
    public FishControl fishOnBoard { get; private set; }
    public bool cuttingBoardTaken => fishOnBoard != null;
    public Camera basketCamera;
    public Transform fishOnCuttingBoardTransform;
    /// the more the worse
    private int score = 0;

    void Awake () {
        fishToCut = generatedAIFishCount;
    }

    public override void OnStartServer () {
        base.OnStartServer();
        StartCoroutine(OnGameStart());
    }

    private IEnumerator OnGameStart () {
        score = 0;
        for (int i = 0; i < generatedAIFishCount; ++i) {
            var aiFish = Instantiate(aiFishPrefab, fishSpawnPoint.position + 0.2f * i * Vector3.up, Quaternion.Euler(0, Random.Range(0, 360), 0));
            NetworkServer.Spawn(aiFish);
            fishList.Add(aiFish.GetComponent<FishControl>());
        }

        yield return null;
        yield return null;
        // NOTE wait till local player is ready
        moveFishToCuttingBoard(fishList[0]);
    }

    [Server]
    public void OnPlayerJoin (PlayerFishController player) {
        fishList.Add(player.GetComponent<FishControl>());
    }

    [Server]
    public void NextFish () {
        // TODO
        // change the current fish to next fish on basket
        // might just be a function for playtesting
    }

    [Server]
    public void moveFishToCuttingBoard(FishControl fish) {
        fishOnBoard = fish;
        fish.RpcMoveTo(fishOnCuttingBoardTransform.position);
    }

    [Server]
    public void moveFishBackToBasket(FishControl fish) {
        fishOnBoard = null;
        fish.RpcMoveTo(fishSpawnPoint.position);
    }

    public void OnFishKilled (FishControl fish) {
        if (fishOnBoard == fish) fishOnBoard = null;
        fishList.Remove(fish);
        if (fish.GetComponent<AIFishController>() != null) {
            fishToCut--;
        } else {
            score += 10;
        }
        if (fishToCut > 0) {
           if (fishList.Count > 0) {
               moveFishToCuttingBoard(fishList[0]);
           }
        } else {
            // TODO game over, show score
        }
    }

    public void OnMercyFish (FishControl fish) {
        fishList.Remove(fish);
        if (fishOnBoard == fish) fishOnBoard = null;
        if (fishList.Count > 0) {
            moveFishToCuttingBoard(fishList[0]);
        }
        score++;
    }

    [ServerCallback] // TODO might also show gui on clients (fish view)
    void OnGUI () {
        GUI.Label(new Rect(20, 20, 300, 80), $"{fishToCut} more fishes to kill | score: -{score}");
    }
}
