namespace OnlineService {
    using UnityEngine;
    using System.Collections.Generic;

    public class MultiplayerServiceDummy : IMultiplayerService {
        private float createLobbyTime;
        private Promise<ILobby> createPromise;
        private Promise<List<ILobbySnapshot>> requestPromise;
        private float requestLobbyTime;

        private class FakeLobby : ILobby {
            public string name { get; private set; }
            public LobbyType type { get; private set; }
            public bool isHost { get; private set; }
            public FakeLobby (string name, LobbyType type, bool isHost) {
                this.name = name;
                this.type = type;
                this.isHost = isHost;
            }
            public Future Leave () {
                var promise = new Promise();
                promise.Error("not implemented"); // TODO
                return promise;
            }
            public Future Lock () {
                var promise = new Promise();
                promise.Return();
                return promise;
            }
        }

        private class FakeLobbySnapshot : ILobbySnapshot {
            public string name { get; set; }
            public LobbyType type { get; set; }
            public int currentSize { get; set; }
            public bool hasPassword { get; set; }
            public Future<ILobby> Join (string password) {
                var promise = new Promise<ILobby>();
                promise.Error("not implemented"); // TODO
                return promise;
            }
        }

        private FakeLobby currentLobby;
        private List<ILobbySnapshot> requestList;

        public void Activate () {}
        public void Deactivate () {}

        public void Update() {
            if (createPromise != null) {
                if (Time.time > createLobbyTime + 5) { // simulate 5 second delay
                    NetworkGameManager.singleton.StartHost();
                    createPromise.Return(currentLobby);
                    createPromise = null;
                }
            }
            if (requestPromise != null) {
                if (Time.time > requestLobbyTime + 5) {
                    requestPromise.Return(requestList);
                    requestPromise = null;
                }
            }
        }

        public Future<ILobby> CreateLobby(string name, int size, string userID, LobbyType lobbyType, string password, int domain) {
            this.createPromise = new Promise<ILobby>();
            currentLobby = new FakeLobby(name, lobbyType, true);
            createLobbyTime = Time.time;
            return this.createPromise;
        }

        public Future<List<ILobbySnapshot>> RequestLobbies(int resultSize, string filter, int domain) {
            var list = new List<ILobbySnapshot>();
            for (int i = 0; i < 5; ++i) {
                list.Add(new FakeLobbySnapshot() { name = "Lobby " + i, currentSize = Mathf.Clamp(i - 1, 0, 2), hasPassword = (i == 2) });
            }
            requestList = list;
            this.requestPromise = new Promise<List<ILobbySnapshot>>();
            requestLobbyTime = Time.time;
            return requestPromise;
        }

        public Future<List<ILobbySnapshot>> RequestUserLobbies(List<string> activeFriendList, bool includeInviteOnly, int domain) {
            var promise = new Promise<List<ILobbySnapshot>>();
            promise.Return(new List<ILobbySnapshot>());// not supported
            return promise;
        }
    }
}
