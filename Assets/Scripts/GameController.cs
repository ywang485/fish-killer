using UnityEngine;
using UnityEngine.UI;
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

    public ChefScreen chefScreen;
    public ScoreScreen finalScoreScreen;
    public Text scoreTextUI;
    public GameObject fishWinScreen;
    public GameObject fishLoseScreen;

    public GameObject aiFishPrefab;
    public Camera basketCamera;
    public Transform fishOnCuttingBoardTransform;

    [Header("Spawn")]
    public Transform fishSpawnPoint;
    /// put players in a secret place first when they join in case the chef cheats
    public Transform playerSpawnPoint;
    [Header("Parameter")]
    public int generatedAIFishCount;

    public int fishToKill { get; private set; } // reserved if fishToKill might not be designed as the count of AI fishes
    public List<FishControl> fishList { get; } = new List<FishControl>();
    public FishControl fishOnBoard { get; private set; }
    public bool cuttingBoardTaken => fishOnBoard != null;

    public int fishKilled { get; private set; }
    public int allFishMercied { get; private set; }
    public int playersKilled { get; private set; }
    public int playersMercied { get; private set; }

    public bool gameover { get; private set; }

    void Awake () {
        fishToKill = generatedAIFishCount;
    }

    public override void OnStartServer () {
        base.OnStartServer();
        StartCoroutine(OnGameStart());
    }

    private IEnumerator OnGameStart () {
        fishKilled = 0;
        allFishMercied = 0;
        playersMercied = 0;
        playersKilled = 0;

        for (int i = 0; i < generatedAIFishCount; ++i) {
            SpawnAIFish(0.2f * i * Vector3.up);
        }

        yield return null;
        yield return null;
        // NOTE wait till local player is ready
        moveFishToCuttingBoard(fishList[0]);
    }

    private void SpawnAIFish (Vector3 spawnOffset = default(Vector3)) {
        var aiFish = Instantiate(aiFishPrefab, fishSpawnPoint.position + spawnOffset, Quaternion.Euler(0, Random.Range(0, 360), 0));
        NetworkServer.Spawn(aiFish);
        fishList.Add(aiFish.GetComponent<FishControl>());
    }

    void Update () {
        if (isServer) {
            scoreTextUI.text = $"{fishToKill} more fishes to kill | {allFishMercied} are mercied.";
        }
    }

    [Server]
    public void OnPlayerJoin (PlayerFishController player) {
        fishList.Insert(0, player.GetComponent<FishControl>());
    }

    [Server]
    public void ResetPlayer (PlayerFishController player) {
        fishList.Insert(Random.Range(0, fishList.Count), player.GetComponent<FishControl>());
    }

    [Server]
    public void NextFish () {
        if (fishOnBoard != null) {
            throw new System.InvalidOperationException("Shouldn't be called when fish on board is non-null");
        }
        if (fishToKill > 0) {
            if (fishList.Count > 0) {
                while (fishList[0] == null && fishList.Count > 0) {
                    fishList.RemoveAt(0);
                    CheckSpawnAIFish();
                }
                moveFishToCuttingBoard(fishList[0]);
            }
        } else {
            GameOver(true);
        }
    }

    [Server]
    public void moveFishToCuttingBoard(FishControl fish) {
        fishOnBoard = fish;
        fish.RpcMoveTo(fishOnCuttingBoardTransform.position, true);
    }

    [Server]
    public void moveFishBackToBasket(FishControl fish) {
        fishOnBoard = null;
        fish.RpcMoveTo(fishSpawnPoint.position, false);
    }

    private void RemoveFishFromList (FishControl fish) {
        fishList.Remove(fish);
        CheckSpawnAIFish();

        while (fishList.Count > 0 && fishList[0] == null) { // in case player leaves and the object is destroyed
            fishList.RemoveAt(0);
        }
    }

    private void CheckSpawnAIFish () { // ensure there are always some fish in the basket
        var fishCountToFill = Random.Range(2, 5);
        int k = 0;
        while (fishList.Count < fishCountToFill) {
            SpawnAIFish(0.2f * (k++) * Vector3.up);
        }
    }

    public void OnFishKilled (FishControl fish) {
        if (fishOnBoard == fish) fishOnBoard = null;
        RemoveFishFromList(fish);
        fishKilled++;
        if (fish.GetComponent<AIFishController>() != null) {
            fishToKill--;

            NextFish();
        } else {
            GameOver(false);
            playersKilled++;
        }
    }

    public void OnMercyFish (FishControl fish) {
        RemoveFishFromList(fish);
        if (fishOnBoard == fish) fishOnBoard = null;
        if (fishList.Count > 0) {
            moveFishToCuttingBoard(fishList[0]);
        }
        if (fish.GetComponent<PlayerFishController>() != null) playersMercied++;
        allFishMercied++;
    }

    private void GameOver (bool success) {
        chefScreen.normalScreen.SetActive(false);
        finalScoreScreen.gameObject.SetActive(true);
        finalScoreScreen.Show(this, success);

        gameover = true;
    }

    public void Restart () {
        chefScreen.normalScreen.SetActive(true);
        finalScoreScreen.gameObject.SetActive(false);

        fishToKill = generatedAIFishCount;

        Debug.Assert(fishList.Count > 0);
        moveFishToCuttingBoard(fishList[0]);

        fishKilled = 0;
        allFishMercied = 0;
        playersKilled = 0;
        playersMercied = 0;

        gameover = false;
    }
}
