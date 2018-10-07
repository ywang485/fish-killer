namespace OnlineService {
    using System.Collections.Generic;

    public enum LobbyType {
        InviteOnly,
        FriendsOnly,
        Public
    }

    static public class MultiplayerServiceManager {
        static private IMultiplayerService _service;
        static MultiplayerServiceManager () {}
        static public IMultiplayerService GetService () {
            if (_service == null) {
                _service = new UnetMultiplayerService(null);
            }
            return _service;
        }

        static public void Update () {
            if (_service != null) _service.Update();
        }

        static public void Reset () {
            if (_service != null) _service.Deactivate();
        }
    }

    public interface IMultiplayerService {
        void Activate();
        void Deactivate();
        /// @remark callback backs may be called in this function. All future callbacks should be in the same thread.
        void Update();
        /// @arg callback argument is null on fail
        Future<ILobby> CreateLobby(string name, int size, string userID, LobbyType lobbyType, string password, int domain);
        /// @arg domain only show rooms of the domain requested. Use this value to silo different incompatible client versions
        Future<List<ILobbySnapshot>> RequestLobbies(int resultSize, string filter, int domain);
        /// @arg callback argument is null on fail
        Future<List<ILobbySnapshot>> RequestUserLobbies(List<string> activeFriendList, bool includeInviteOnly, int domain);
    }

    public interface ILobbySnapshot {
        string name { get; }
        LobbyType type { get; }
        /// the current number of players in the room
        int currentSize { get; }
        bool hasPassword { get; }
        /// @arg callback argument will be null on failure
        Future<ILobby> Join (string password);
    }

    public interface ILobby {
        string name { get; }
        bool isHost { get; }
        /// make it not accepting new clients to join
        Future Lock();
        Future Leave();
    }

    public interface IConnectionSurrogate {
        void Activate ();
        void Deactivate ();
        /// @remark callback backs may be called in this function. All future callbacks should be in the same thread.
        void Update ();
        /// respond with custom data sent to clients
        Future<string> GetHostData (string hostAddr, int hostPort);
        /// @arg callback respond with "address:port"
        Future<string> GetJoinAddress (string hostData);
    }

    public class ConnectionSurrogateDummy : IConnectionSurrogate {
        public void Activate () {}
        public void Deactivate () {}
        public void Update () {}
        public Future<string> GetHostData (string hostAddr, int hostPort) {
            var promise = new Promise<string>();
            promise.Return(hostAddr + ":" + hostPort);
            return promise;
        }
        public Future<string> GetJoinAddress (string hostData) {
            var promise = new Promise<string>();
            promise.Return(hostData);
            return promise;
        }
    }
}
