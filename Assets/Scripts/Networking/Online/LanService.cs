namespace LanService {
    using OnlineService;
    using UnityEngine.Networking;

    public class LocalLobby : ILobbySnapshot, ILobby  {
        public readonly string address;
        public readonly int port;
        public string name {
            get {
                return address + ":" + port;
            }
        }
        public LobbyType type { get { return LobbyType.Public; } }
        public int currentSize { get { return 0; } }
        public bool hasPassword { get { return false; } }
        public bool isHost {
            get {
                return false;
            }
        }

        public LocalLobby (string address, int port) {
            this.address = address;
            this.port = port;
        }

        public Future<ILobby> Join (string password) {
            NetworkGameManager.singleton.networkAddress = address;
            NetworkGameManager.singleton.networkPort = port;
            NetworkGameManager.singleton.StartClient();
            NetworkGameManager.ConnectionStatusHandler handler = null;
            var promise = new Promise<ILobby>();
            handler = (success) => {
                if (success) promise.Return(this);
                else promise.Error(null);
                NetworkGameManager.instance.onClientConnectionStatusChanged -= handler;
            };

            NetworkGameManager.instance.onClientConnectionStatusChanged += handler;
            return promise;
        }

        public Future Leave () {
            NetworkGameManager.singleton.StopClient();
            var promise = new Promise();
            promise.Return();
            return promise;
        }

        public Future Lock () {
            var ret = new Promise();
            ret.Error("not the host");
            return ret;
        }
    }

    public class LocalHostLobby : ILobby {
        public string name {
            get {
                return "";
            }
        }

        public LobbyType type { get { return LobbyType.Public; } }

        public bool isHost {
            get {
                return true;
            }
        }

        public Future Leave () {
            NetworkGameManager.singleton.StopHost();
            NetworkGameManager.instance.StopDiscovery();
            var promise = new Promise();
            promise.Return();
            return promise;
        }

        public Future Lock () {
            var promise = new Promise();
            NetworkGameManager.instance.StopDiscovery();
            NetworkGameManager.instance.dontAcceptNewClients = true;
            promise.Return();
            return promise;
        }
    }
}
