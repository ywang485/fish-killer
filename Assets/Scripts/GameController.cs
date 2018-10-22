using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

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

    void Awake () {
        fishToCut = generatedAIFishCount;
    }

    public override void OnStartServer () {
        base.OnStartServer();
        OnGameStart();
    }

    private void OnGameStart () {
        for (int i = 0; i < generatedAIFishCount; ++i) {
            var aiFish = Instantiate(aiFishPrefab, fishSpawnPoint.position + 0.2f * i * Vector3.up, Quaternion.Euler(0, Random.Range(0, 360), 0));
            NetworkServer.Spawn(aiFish);
            fishList.Add(aiFish.GetComponent<FishControl>());
        }
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
        fish.RpcMoveToBoard(fishOnCuttingBoardTransform.position);
    }

    [Server]
    public void moveFishBackToBasket(FishControl fish) {
        fishOnBoard = null;
        fish.RpcMoveTo(fishSpawnPoint.position);
    }

    public void OnFishKilled (FishControl fish) {
        if (fishOnBoard == fish) fishOnBoard = null;
        fishList.Remove(fish);
        fishToCut--;
        if (fishToCut == 0) {
            // TODO game over, show score
        }
    }

    [ServerCallback] // TODO might also show gui on clients (fish view)
    void OnGUI () {
        GUI.Label(new Rect(20, 20, 200, 80), $"{fishToCut} more fishes to kill");
    }
}
